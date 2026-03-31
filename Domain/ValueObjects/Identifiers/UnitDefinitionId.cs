using System;

namespace Domain.ValueObjects.Identifiers;

public record UnitDefinitionId(Guid Value) : Id<UnitDefinitionId>(Value)
{
    public static UnitDefinitionId New() => new(Guid.NewGuid());
}