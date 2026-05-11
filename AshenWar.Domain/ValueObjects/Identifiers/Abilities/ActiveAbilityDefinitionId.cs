using System;

namespace Domain.ValueObjects.Identifiers.Abilities;

public record ActiveAbilityDefinitionId(Guid Value) : Id<ActiveAbilityDefinitionId>(Value)
{
    public static ActiveAbilityDefinitionId New() => new(Guid.NewGuid());
}