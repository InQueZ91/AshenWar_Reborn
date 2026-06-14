using System;

namespace AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

public record AbilityStepId(Guid Value) : LocalId<AbilityStepId>(Value)
{
    public new static AbilityStepId New() => new(Guid.NewGuid());
}