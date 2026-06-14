using System;

namespace AshenWar.Domain.ValueObjects.LocalIdentifiers;

public record ModifierDefinitionId(Guid Value) : LocalId<ModifierDefinitionId>(Value)
{
    public new static ModifierDefinitionId New() => new(Guid.NewGuid());
}