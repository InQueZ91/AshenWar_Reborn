using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Events.Tiles;

public record TileConditionApplied(TileId TileId, TileConditionDefinitionId TileConditionDefinitionId) : IDomainEvent;