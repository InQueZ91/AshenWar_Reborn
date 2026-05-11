using System;

namespace Domain.ValueObjects.Identifiers;

public sealed record StatDefinitionId(Guid Value) : Id<StatDefinitionId>(Value)
{
    public static StatDefinitionId New() => new(Guid.NewGuid());
}