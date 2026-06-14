using System.Collections.Generic;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.ValueObjects.Orders;

public sealed record AbilityOrder
{
    public required AbilityId AbilityId { get; init; }
    public IReadOnlyList<StepSelection> Selections { get; init; } = [];
}