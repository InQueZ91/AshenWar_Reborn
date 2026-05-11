using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Conditions;
using Domain.ValueObjects.Identifiers.Tiles;

namespace Domain.Events.Tiles;

public record TileConditionRemoved(TileId TileId, TileConditionDefinitionId TileConditionDefinitionId) : IDomainEvent;