
using System;

namespace Domain.ValueObjects.Identifiers.Tiles;

public record TileId(Guid Value) : Id<TileId>(Value)
{
    public static TileId New() => new(Guid.NewGuid()); 
}