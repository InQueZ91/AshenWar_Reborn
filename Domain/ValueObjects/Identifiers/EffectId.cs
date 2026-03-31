using System;

namespace Domain.ValueObjects.Identifiers;

public record EffectId(Guid Value) : Id<EffectId>(Value)
{
    public static EffectId New() => new EffectId(Guid.NewGuid());
}
