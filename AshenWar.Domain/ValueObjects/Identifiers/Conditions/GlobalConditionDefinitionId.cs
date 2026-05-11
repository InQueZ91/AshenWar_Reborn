using System;

namespace Domain.ValueObjects.Identifiers.Conditions;

public record GlobalConditionDefinitionId(Guid Value) : Id<GlobalConditionDefinitionId>(Value) 
{
    public static GlobalConditionDefinitionId New() => new(Guid.NewGuid());
}