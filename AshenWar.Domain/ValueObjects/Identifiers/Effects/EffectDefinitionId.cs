using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Effects;

public record EffectDefinitionId(Guid Value) : Id<EffectDefinitionId>(Value)
{
    public static EffectDefinitionId New() => new(Guid.NewGuid());
}