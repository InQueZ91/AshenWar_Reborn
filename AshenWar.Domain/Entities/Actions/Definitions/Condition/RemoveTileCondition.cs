using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.Entities.Actions.Definitions.Condition;

public sealed class RemoveTileCondition : IActionDefinition
{
    public TileConditionDefinitionId TileConditionDefinitionId { get; init; } = null!;
}