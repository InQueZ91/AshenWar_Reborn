using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Conditions;

public record UnitConditionDefinitionId(Guid Value) : Id<UnitConditionDefinitionId>(Value)
{
    public static UnitConditionDefinitionId New() => new(Guid.NewGuid());
}