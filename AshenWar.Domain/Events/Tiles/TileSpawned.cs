using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Tiles;

namespace Domain.Events.Tiles;

public record TileSpawned(TileId TileId, HexCoord Position) : IDomainEvent;