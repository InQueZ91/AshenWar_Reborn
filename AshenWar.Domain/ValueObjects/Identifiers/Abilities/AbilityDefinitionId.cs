using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Abilities;

public record AbilityDefinitionId(Guid Value) : Id<AbilityDefinitionId>(Value)
{
    public static AbilityDefinitionId New() => new(Guid.NewGuid());
}