using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Events;
using Application.Execution;
using Application.Hubs;
using Application.Planning;
using Application.ValueObjects;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Condition;
using Domain.Entities.Actions.ValueSources;
using Domain.Entities.Conditions.Global;
using Domain.Entities.Conditions.Tile;
using Domain.Entities.Conditions.Unit;
using Domain.Entities.Match;
using Domain.Entities.Stats;
using Domain.Exceptions;
using Domain.Interfaces.Conditions;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;
using Microsoft.AspNetCore.SignalR;

namespace Application;

public sealed class ResolutionService(
    IHubContext<GameHub> hubContext,
    IMatchRepository matchRepository,
    PlanningTimerService planningTimerService,
    InitiativeService initiativeService,
    ActiveAbilityExecutor activeAbilityExecutor,
    ConditionExecutor<UnitCondition> unitConditionExecutor,
    ConditionExecutor<TileCondition> tileConditionExecutor,
    ConditionExecutor<GlobalCondition> globalConditionExecutor,
    TriggerChain triggerChain,
    ActionExecutor actionExecutor)
{
    public async Task Resolve(MatchId matchId, PlayerOrders blue, PlayerOrders red, CancellationToken ct)
    {
        // 1 Setup
        var match = await matchRepository.GetMatchAsync(matchId, ct);
        
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
        await hubContext.Clients
            .Group(matchId.Value.ToString())
            .SendAsync("TurnInitiativeRolled", initiative, ct);

        // 4 Ability Resolution
        resolutionBatches.AddRange(await ActiveAbilityResolution(matchContext, orderedUnits, ct));
        
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
        
        var matchOutcome = match.EvaluateOutcome();
        if (matchOutcome.IsOver)
        {
            match.DeclareWinner(matchOutcome.WinnerId);
            await matchRepository.SaveAsync(match, ct);
            await BroadcastResolutionCompleted(matchId, result, ct);
            await BroadcastMatchEnded(matchId, matchOutcome.WinnerId, ct);
        }
        else
        {
            var nextTurn = match.BeginNextTurn();
            nextTurn.BeginPlanning(match.PlanningDuration);
            resolutionBatches.Add(await RunMatchTriggers(match, matchContext, ct));
            
            await matchRepository.SaveAsync(match, ct);
            await planningTimerService.StartPlanningTimer(match.Id, nextTurn, ct);
            await BroadcastResolutionCompleted(matchId, result, ct);
            await BroadcastPlanningStarted(matchId, nextTurn, ct);
        }
    }

    private async Task BroadcastResolutionCompleted(MatchId matchId, TurnResolutionResult result, CancellationToken ct)
    {
        await hubContext.Clients.Group(matchId.Value.ToString())
            .SendAsync("TurnResolutionCompleted", result, ct);
    }

    private async Task BroadcastMatchEnded(MatchId matchId, UserId? winnerId, CancellationToken ct)
    {
        await hubContext.Clients.Group(matchId.Value.ToString())
            .SendAsync("MatchEnded", winnerId, ct);
    }

    private async Task BroadcastPlanningStarted(MatchId matchId, Turn nextTurn, CancellationToken ct)
    {
        await hubContext.Clients.Group(matchId.Value.ToString())
            .SendAsync("PlanningStarted", new
            {
                nextTurn.Id,
                nextTurn.TurnNumber,
                nextTurn.PlanningStartedAt,
                nextTurn.PlanningDuration,
                ServerNow = DateTimeOffset.UtcNow
            }, ct);
    }

    private async Task<ResolutionBatch> RunMatchTriggers(Match match, MatchContext matchContext, CancellationToken ct)
    {
        var collector = new DomainEventCollector();
    
        foreach (var evt in match.FlushDomainEvents())
            collector.Collect(evt);
    
        foreach (var evt in match.CurrentTurn.FlushDomainEvents())
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
                Stacks = new FixedValueSource(stacks)
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
            .OfType<IConditionCommand<TileCondition>?>()
            .Where(u => u != null && u.Conditions.Any())
            .ToList();

        foreach (var holder in tileWithCondition.OfType<IConditionCommand<TileCondition>>())
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
            .OrderByDescending(u => u.GetMaxStat(StatDefinition.Speed))
            .Select(u => u as IConditionCommand<UnitCondition>)
            .ToList();

        foreach (var holder in unitWithCondition.OfType<IConditionCommand<UnitCondition>>())
        {
            var onTickResolutionBatch = await unitConditionExecutor.Tick(holder, matchContext, ct);
            
            result.Add(onTickResolutionBatch);
        }
        
        return result;
    }
    
    private async Task<List<ResolutionBatch>> ActiveAbilityResolution(MatchContext matchContext,
        List<UnitOrder> orderedUnits,
        CancellationToken ct)
    {
        var result = new List<ResolutionBatch>();
        var match = matchContext.Match;
        
        foreach (var unitOrder in orderedUnits)
        {
            var unit = match.GetUnitById(unitOrder.UnitId);
            if (unit is null)
            {
                // Raise application event
                continue;
            }
            
            var ability = unit.GetActiveAbilityById(unitOrder.AbilityId);
            if (ability is null)
            {
                // Raise application event
                continue;
            }

            var resolutionBatch = await activeAbilityExecutor.Execute(
                unit,
                ability,
                unitOrder.PhaseInputs.ToList(),
                matchContext,
                ct
            );
            
            result.Add(resolutionBatch);
        }

        return result;
    }
}