using System;

namespace Domain.ValueObjects.LocalIdentifiers.Abilities;

public record ActiveAbilityId(Guid Value) : LocalId<ActiveAbilityId>(Value)
{
    public new static ActiveAbilityId New() => new(Guid.NewGuid());
}