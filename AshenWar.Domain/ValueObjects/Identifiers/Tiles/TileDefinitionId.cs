using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Tiles;

public record TileDefinitionId(Guid Value) : Id<TileDefinitionId>(Value)
{
    public static TileDefinitionId New() => new(Guid.NewGuid()); 
}