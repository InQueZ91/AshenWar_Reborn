using System.Collections.Generic;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.ValueObjects.Orders;

public sealed record UnitOrder
{
    public required UnitId UnitId { get; init; }
    public IReadOnlyList<AbilityOrder> AbilityOrders { get; init; } = [];
}