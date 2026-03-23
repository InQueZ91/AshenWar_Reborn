using System;

namespace Domain.ValueObjects.Identifiers;

public readonly record struct UnitDefinitionId(Guid Value)
{
    public static UnitDefinitionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}