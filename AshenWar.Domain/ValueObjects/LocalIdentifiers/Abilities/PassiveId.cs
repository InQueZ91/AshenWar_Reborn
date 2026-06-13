using System;

namespace AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

public record PassiveId(Guid Value) : LocalId<PassiveId>(Value) 
{
    public new static PassiveId New() => new(Guid.NewGuid());
}