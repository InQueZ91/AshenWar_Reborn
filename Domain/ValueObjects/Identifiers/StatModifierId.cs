using System;

namespace Domain.ValueObjects.Identifiers;

public record StatModifierId(Guid Value) : Id<StatModifierId>(Value)
{
    public static StatModifierId New() => new(Guid.NewGuid());
}