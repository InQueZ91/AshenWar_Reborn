using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Constants;
using Application.Contracts;
using Application.ValueObjects;
using Domain.Entities.Match;
using Domain.Enums.Conditions;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;

namespace Application.Planning;

public sealed class PlanningService(
    IMatchRepository matchRepository,
    ResolutionService resolutionService,
    PlanningTimerService planningTimerService)
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
            var match = await matchRepository.GetMatchAsync(matchId, ct);
            
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
                var unit = match.GetUnitById(unitOrder.UnitId);
                if (unit is null)
                    return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} not found.");

                if (unit.Owner != userId)
                    return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} does not belong to player {userId}.");

                var ability = unit.GetActiveAbilityById(unitOrder.AbilityId);
                if (ability is null)
                    return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} does not have ability {unitOrder.AbilityId}");

                if (!ability.IsReady)
                    return SubmitOrdersResult.Fail($"Ability {unitOrder.AbilityId} not ready.");
                
                // Validate conditions
                var disableConditions = new[]
                {
                    ConditionTag.Exhaust,
                    ConditionTag.Rest,
                    ConditionTag.Stun
                };
                if (unit.HasAnyConditionWithTag(disableConditions))
                    return SubmitOrdersResult.Fail($"Unit {unitOrder.UnitId} is disabled.");

                // Validate phase inputs
                foreach (var phase in ability.Definition.Phases.Where(p => p.Shape?.RequiresInput ?? false))
                {
                    var input = unitOrder.PhaseInputs.FirstOrDefault(i => i.AbilityPhaseId == phase.Id);
                    if (input?.SelectedTarget is null)
                        return SubmitOrdersResult.Fail($"Phase {phase.Id} requires input target.");
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
                planningTimerService.CancelTimer();
                
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