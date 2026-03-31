using System;

namespace Domain.ValueObjects.Identifiers;

public record AbilityDefinitionId(Guid Value) : Id<AbilityDefinitionId>(Value)
{
    public static AbilityDefinitionId New() => new(Guid.NewGuid());
}