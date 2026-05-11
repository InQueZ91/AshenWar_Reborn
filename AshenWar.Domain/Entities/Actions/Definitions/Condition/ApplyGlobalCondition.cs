using Domain.Entities.Actions.ValueSources;
using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.Entities.Actions.Definitions.Condition;

public sealed class ApplyGlobalCondition : IActionDefinition
{
    public GlobalConditionDefinitionId GlobalConditionDefinitionId { get; init; } = null!;
    public ValueSource Stacks { get; init; } = ValueSource.Fixed(1);
}