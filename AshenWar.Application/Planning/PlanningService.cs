using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AshenWar.Application.Constants;
using AshenWar.Application.Contracts;
using AshenWar.Application.Services;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Orders;
using Microsoft.Extensions.Logging;

namespace AshenWar.Application.Planning;

public sealed class PlanningService(
    ChannelWriter<TimerMessage> writer,
    IMatchRepository matchRepository,
    ResolutionService resolutionService,
    ILogger<PlanningService> logger)
{
    private readonly ConcurrentDictionary<MatchId, SemaphoreSlim> _locks = new();
    
    public async Task<SubmitOrdersResult> SubmitOrders(
        UserId userId,
        MatchId matchId,
        List<UnitOrder> orders,
        CancellationToken ct)
    {
        var matchLock = _locks.GetOrAdd(matchId, _ => new SemaphoreSlim(1,1));
        await matchLock.WaitAsync(ct);
        try
        {
            var match = await matchRepository.FindAsync(matchId, ct);
            
            if (match is null)
                return SubmitOrdersResult.Fail("Match not found.");

            // Validate player belongs to match
            if (match.BlueSide.UserId != userId && match.RedSide.UserId != userId)
                return SubmitOrdersResult.Fail("Player does not belong to this match.");

            // Validate orders not already submitted
            var turn = match.CurrentTurn;
            var isBlue = match.BlueSide.UserId == userId;
            switch (isBlue)
            {
                case true when turn.BlueSubmitted:
                case false when turn.RedSubmitted:
                    return SubmitOrdersResult.Fail("Orders already submitted.");
            }

            // Validate deadline
            var deadline = turn.PlanningStartedAt + turn.PlanningDuration + PlanningConstants.GracePeriod;
            if (DateTimeOffset.UtcNow > deadline)
                return SubmitOrdersResult.Fail("Planning phase has ended.");

            // Validate unit orders
            foreach (var unitOrder in orders)
            {
                // Validate unit exists
                var unit = match.Board.FindUnitById(unitOrder.UnitId);
                if (unit is null)
                    return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} not found.");

                // Validate unit belongs to player
                if (unit.Owner != userId)
                    return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} does not belong to player {userId}.");

                // Validate unit conditions
                var disableConditions = new[]
                {
                    ConditionTag.Exhaust,
                    ConditionTag.Rest,
                    ConditionTag.Stun
                };
                if (unit.HasAnyConditionWithTag(disableConditions))
                    return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} is disabled.");
                
                // Validate ability orders
                foreach (var abilityOrder in unitOrder.AbilityOrders)
                {
                    // Validate ability exists
                    var ability = unit.GetAbilityById(abilityOrder.AbilityId);
                    if (ability is null)
                        return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} does not have ability {abilityOrder.AbilityId}");

                    // Validate ability is ready
                    if (!ability.IsReady)
                        return SubmitOrdersResult.Fail($"Ability {abilityOrder.AbilityId} not ready.");

                    // Validate ability step
                    foreach (var step in ability.Definition.Steps.Where(p => p.Shape.RequiresInput))
                    {
                        var input = abilityOrder.Selections.FirstOrDefault(i => i.AbilityStepId == step.Id);
                        if (input?.Target is null)
                            return SubmitOrdersResult.Fail($"Phase {step.Id} requires input target.");
                    }
                }
            }

            // Submit orders
            var playerOrders = PlayerOrders.Create(userId, orders);
            if (isBlue)
                turn.SubmitBlueOrders(playerOrders);
            else
                turn.SubmitRedOrders(playerOrders);

            await matchRepository.SaveAsync(match, ct);

            // Fire and forget - client gets result via SignalR
            if (turn.BothSubmitted)
            {
                if (!writer.TryWrite(new CancelTimer(matchId)))
                    logger.LogWarning("Failed to write timer message for match {MatchId}", matchId);
                
                _ = Task.Run(() => resolutionService.Resolve(
                        matchId,
                        turn.BlueOrders,
                        turn.RedOrders,
                        CancellationToken.None),
                    CancellationToken.None);
            }

            return SubmitOrdersResult.Ok();
        }
        finally
        {
            matchLock.Release();
        }
    }
}