using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Domain.Entities.Actions.Definitions.Condition;

public sealed record ApplyGlobalCondition : IActionDefinition
{
    public required GlobalConditionDefinitionId GlobalConditionDefinitionId { get; init; }
    public ValueSource Stacks { get; init; } = ValueSource.Fixed(1);
}