using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Abilities;

public record PassiveDefinitionId(Guid Value) : Id<PassiveDefinitionId>(Value)
{
    public static PassiveDefinitionId New() => new(Guid.NewGuid());
}