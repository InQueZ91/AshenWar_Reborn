using System;
using System.Linq;
using AshenWar.Application.Events;
using AshenWar.Application.History.Records;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Orders;

namespace AshenWar.Application.History;

public sealed class TurnRecordFactory
{
    public TurnRecord Create(MatchId matchId, TurnResolutionResult result, PlayerOrders blue, PlayerOrders red)
    {
        return new TurnRecord(
            matchId.Value,
            result.TurnId.Value,
            result.TurnNumber,
            result.Initiative.Seed,
            result.Initiative.FinalOrder.Select(id => id.Value).ToList(),
            [ToOrdersRecord(blue), ToOrdersRecord(red)],
            result.Batches,
            DateTimeOffset.UtcNow);
    }

    private static PlayerOrderRecord ToOrdersRecord(PlayerOrders orders)
        => new(orders.Owner.Value, orders.Orders.Select(ToUnitOrderRecord).ToList());

    private static UnitOrderRecord ToUnitOrderRecord(UnitOrder order)
    {
        return new UnitOrderRecord
        {
            UnitId = order.UnitId.Value,
            AbilityOrders = order.AbilityOrders.Select(ao => new AbilityOrderRecord
            {
                AbilityId = ao.AbilityId.Value,
                StepSelections = ao.Selections.Select(ToStepSelectionRecord).ToList()
            }).ToList()
        };
    }

    private static StepSelectionRecord ToStepSelectionRecord(StepSelection selection)
        => selection.Target switch
        {
            Target.Unit u => new StepSelectionRecord(selection.AbilityStepId.Value, TargetType.Unit, u.Id.Value),
            Target.Tile t => new StepSelectionRecord(selection.AbilityStepId.Value, TargetType.Tile, t.Id.Value),
            Target.Self => new StepSelectionRecord(selection.AbilityStepId.Value, TargetType.Self, null),
            _ => throw new InvalidOperationException($"Unknown Target type: {selection.Target.GetType().Name}")
        };
}