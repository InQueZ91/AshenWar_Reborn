using System;

namespace Domain.ValueObjects.LocalIdentifiers.Abilities;

public record AbilityPhaseId(Guid Value) : LocalId<AbilityPhaseId>(Value)
{
    public new static AbilityPhaseId New() => new(Guid.NewGuid());
}