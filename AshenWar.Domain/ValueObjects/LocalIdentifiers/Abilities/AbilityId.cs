using System;

namespace AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

public record AbilityId(Guid Value) : LocalId<AbilityId>(Value)
{
    public new static AbilityId New() => new(Guid.NewGuid());
}