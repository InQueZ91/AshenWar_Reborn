using System;

namespace Domain.ValueObjects.Identifiers.Conditions;

public record TileConditionDefinitionId(Guid Value) : Id<TileConditionDefinitionId>(Value)
{
    public static TileConditionDefinitionId New() => new(Guid.NewGuid());
}