using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Notifications;
using AshenWar.Application.Events;
using AshenWar.Application.Execution;
using AshenWar.Application.History;
using AshenWar.Application.Planning;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;
using AshenWar.Domain.Entities.Actions.ValueSources;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Orders;
using Microsoft.Extensions.Logging;

namespace AshenWar.Application.Services;

public sealed class ResolutionService(
    ChannelWriter<TimerMessage> writer,
    ILogger<ResolutionService> logger,
    IGameNotifier gameNotifier,
    IMatchRepository matchRepository,
    IMatchHistoryRepository matchHistoryRepository,
    TurnRecordFactory turnRecordFactory,
    InitiativeService initiativeService,
    AbilityExecutor abilityExecutor,
    ConditionExecutor<UnitCondition> unitConditionExecutor,
    ConditionExecutor<TileCondition> tileConditionExecutor,
    ConditionExecutor<GlobalCondition> globalConditionExecutor,
    TriggerChain triggerChain,
    ActionExecutor actionExecutor)
{
    public async Task Resolve(MatchId matchId, PlayerOrders blue, PlayerOrders red, CancellationToken ct)
    {
        // 1 Setup
        var match = await matchRepository.FindAsync(matchId, ct);
        
        if (match is null)
            throw new DomainException("Match not found.");
        
        var matchContext = new MatchContext(match, match.TriggerRegistry, match.CurrentTurn.TurnNumber);
        var turnSeed = match.Id.GetHashCode() + match.CurrentTurn.TurnNumber;
        var rng = new Random(turnSeed);
        
        // 2 Begin resolution
        match.BeginResolution();
        var resolutionBatches = new List<ResolutionBatch>();
        resolutionBatches.Add(await RunMatchTriggers(match, matchContext, ct));
        
        // 3 Initiative
        var (initiative, orderedUnits) = initiativeService.Build(blue, red, match, turnSeed, rng);
        await gameNotifier.TurnInitiativeRolled(matchId, initiative, ct);

        // 4 Ability Resolution
        resolutionBatches.AddRange(await AbilityResolution(matchContext, orderedUnits, ct));
        
        // 5 Unit Conditions
        resolutionBatches.AddRange(await UnitConditionResolution(match, matchContext, ct));
        
        // 6 Tile Conditions
        resolutionBatches.AddRange(await TileConditionResolution(match, matchContext, ct));
        
        // 7 Global Conditions
        resolutionBatches.AddRange(await GlobalConditionResolution(match, matchContext, ct));
        
        // 8 Global Events
        resolutionBatches.Add(await GlobalEventApplication(rng, match, matchContext, ct));
        
        // 9 End resolution
        var resolvedTurn = match.CurrentTurn;
        match.EndResolution();
        resolutionBatches.Add(await RunMatchTriggers(match, matchContext, ct));
        
        // 10 Build result
        var result = new TurnResolutionResult(
            resolvedTurn.Id,
            resolvedTurn.TurnNumber,
            initiative,
            resolutionBatches
        );
        
        // 11 Append to history
        var turnRecord = turnRecordFactory.Create(matchId, result, blue, red);
        await matchHistoryRepository.AppendTurnAsync(matchId, turnRecord, ct);
        
        // 12 Evaluate outcome and notify
        var matchOutcome = match.EvaluateOutcome();
        if (matchOutcome.IsOver)
        {
            match.DeclareWinner(matchOutcome.WinnerId);
            
            // 1. Persist domain state
            await matchRepository.SaveAsync(match, ct);
            
            // 2. Close history
            await matchHistoryRepository.CloseAsync(matchId, matchOutcome.WinnerId, ct);
            
            // 3. Notify clients
            await gameNotifier.ResolutionCompleted(matchId, result, ct);
            await gameNotifier.MatchEnded(matchId, matchOutcome.WinnerId, ct);
        }
        else
        {
            var nextTurn = match.BeginNextTurn();
            nextTurn.BeginPlanning(match.PlanningDurationSeconds);
            resolutionBatches.Add(await RunMatchTriggers(match, matchContext, ct));
            
            await matchRepository.SaveAsync(match, ct);
            
            if (!writer.TryWrite(new StartTimer(matchId, nextTurn.PlanningStartedAt, nextTurn.PlanningDuration)))
                logger.LogWarning("Failed to write timer message for match {MatchId}", matchId);
            
            await gameNotifier.ResolutionCompleted(matchId, result, ct);
            await gameNotifier.PlanningStarted(matchId, nextTurn, ct);
        }
    }
    
    private async Task<ResolutionBatch> RunMatchTriggers(Match match, MatchContext matchContext, CancellationToken ct)
    {
        var collector = new DomainEventCollector();
    
        foreach (var evt in match.DrainDomainEvents())
            collector.Collect(evt);
    
        foreach (var evt in match.CurrentTurn.DrainDomainEvents())
            collector.Collect(evt);
    
        return await triggerChain.Run(collector, matchContext, ct);
    }

    private async Task<ResolutionBatch> GlobalEventApplication(Random rng,
        Match match,
        MatchContext matchContext,
        CancellationToken ct)
    {
        var globalEvents = match.GlobalEvents
            .Where(e => e.TurnNumber == match.CurrentTurn.TurnNumber);
        
        var collector = new DomainEventCollector();
        
        foreach (var (turnNumber, stacks, pool) in globalEvents)
        {
            var pick = pool[rng.Next(pool.Count)];
            var action = new ApplyGlobalCondition
            {
                GlobalConditionDefinitionId = pick,
                Stacks = new Fixed(stacks)
            };

            var actionContext = new ActionContext(
                WorldSource.Instance,
                new List<ITargetable>(),
                new HashSet<EntityTag>(),
                matchContext
            );
            
            await actionExecutor.Execute(action, actionContext, collector, ct);
        }

        return await triggerChain.Run(collector, matchContext, ct);
    }
    
    private async Task<List<ResolutionBatch>> GlobalConditionResolution(Match match,
        MatchContext matchContext,
        CancellationToken ct)
    {
        var result = new List<ResolutionBatch>();
        
        var onTickResolutionBatch = await globalConditionExecutor.Tick(match, matchContext, ct);

        result.Add(onTickResolutionBatch);

        return result;
    }
    
    private async Task<List<ResolutionBatch>> TileConditionResolution(Match match,
        MatchContext matchContext,
        CancellationToken ct)
    {
        var result = new List<ResolutionBatch>();
        
        var tileWithCondition = match.Board
            .GetAllTiles()
            .OfType<ICondition<TileCondition>?>()
            .Where(u => u != null && u.Conditions.Any())
            .ToList();

        foreach (var holder in tileWithCondition.OfType<ICondition<TileCondition>>())
        {
            var onTickResolutionBatch = await tileConditionExecutor.Tick(holder, matchContext, ct);

            result.Add(onTickResolutionBatch);
        }

        return result;
    }
    
    private async Task<List<ResolutionBatch>> UnitConditionResolution(Match match,
        MatchContext matchContext,
        CancellationToken ct)
    {
        var result = new List<ResolutionBatch>();
        
        var unitWithCondition = match.Board
            .GetAllUnits()
            .Where(u => u.Conditions.Any())
            .OrderByDescending(u => u.GetFinalStat(StatDefinition.Speed))
            .Select(ICondition<UnitCondition> (u) => u)
            .ToList();

        foreach (var holder in unitWithCondition.OfType<ICondition<UnitCondition>>())
        {
            var onTickResolutionBatch = await unitConditionExecutor.Tick(holder, matchContext, ct);
            
            result.Add(onTickResolutionBatch);
        }
        
        return result;
    }
    
    private async Task<List<ResolutionBatch>> AbilityResolution(MatchContext matchContext,
        List<UnitOrder> orderedUnits,
        CancellationToken ct)
    {
        var result = new List<ResolutionBatch>();
        var match = matchContext.Match;
        
        foreach (var unitOrder in orderedUnits)
        {
            var unit = match.Board.FindUnitById(unitOrder.UnitId);
            if (unit is null)
            {
                // Raise application event
                continue;
            }

            foreach (var abilityOrder in unitOrder.AbilityOrders)
            {
                var ability = unit.GetAbilityById(abilityOrder.AbilityId);
                if (ability is null)
                {
                    // Raise application event
                    continue;
                }

                var resolutionBatch = await abilityExecutor.Execute(
                    unit,
                    ability,
                    abilityOrder.Selections.ToList(),
                    matchContext,
                    ct
                );
                
                result.Add(resolutionBatch);
            }
        }

        return result;
    }
}