using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Domain.Entities.Actions.Definitions.Condition;

public sealed record RemoveTileCondition(TileConditionDefinitionId TileConditionDefinitionId) : IActionDefinition;