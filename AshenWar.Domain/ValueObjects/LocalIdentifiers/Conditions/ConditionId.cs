using System;

namespace AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

public sealed record ConditionId(Guid Value) : LocalId<ConditionId>(Value)
{
    public new static ConditionId New() => new(Guid.NewGuid());
} 