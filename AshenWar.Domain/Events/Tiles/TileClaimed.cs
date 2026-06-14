using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Events.Tiles;

public sealed record TileClaimed(TileId TileId) : IDomainEvent;