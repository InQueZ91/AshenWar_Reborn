using System;

namespace Domain.ValueObjects.LocalIdentifiers.Abilities;

public record PassiveAbilityId(Guid Value) : LocalId<PassiveAbilityId>(Value) 
{
    public new static PassiveAbilityId New() => new(Guid.NewGuid());
}