using System;
using System.Collections.Generic;

namespace AshenWar.Application.History.Records;

public sealed record UnitOrderRecord
{
    public Guid UnitId { get; init; }
    public IReadOnlyList<AbilityOrderRecord> AbilityOrders { get; init; } = [];
}