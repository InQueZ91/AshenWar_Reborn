using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Domain.Entities.Actions.Definitions.Condition;

public sealed record ApplyTileCondition : IActionDefinition
{
    public required TileConditionDefinitionId TileConditionDefinitionId { get; init; }
    public ValueSource Stacks { get; init; } = ValueSource.Fixed(1);
}