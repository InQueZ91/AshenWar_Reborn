using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Events.Tiles;

public record TileDespawned(TileId TileId) : IDomainEvent;