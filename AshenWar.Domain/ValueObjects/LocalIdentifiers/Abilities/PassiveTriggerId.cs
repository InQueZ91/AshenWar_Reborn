using System;

namespace AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

public record PassiveTriggerId(Guid Value) : LocalId<PassiveTriggerId>(Value)
{
    public new static PassiveTriggerId New() => new(Guid.NewGuid());
}