using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Tiles;

namespace Domain.Events.Tiles;

public record TileDespawned(TileId TileId) : IDomainEvent;