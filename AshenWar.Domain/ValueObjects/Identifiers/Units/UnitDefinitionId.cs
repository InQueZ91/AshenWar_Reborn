using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Units;

public record UnitDefinitionId(Guid Value) : Id<UnitDefinitionId>(Value)
{
    public static UnitDefinitionId New() => new(Guid.NewGuid());
}