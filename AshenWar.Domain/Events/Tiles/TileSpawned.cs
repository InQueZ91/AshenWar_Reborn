using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Events.Tiles;

public record TileSpawned(TileId TileId, HexCoord Position) : IDomainEvent;