using System;

namespace Domain.ValueObjects.Identifiers.Abilities;

public record PassiveAbilityDefinitionId(Guid Value) : Id<PassiveAbilityDefinitionId>(Value)
{
    public static PassiveAbilityDefinitionId New() => new(Guid.NewGuid());
}