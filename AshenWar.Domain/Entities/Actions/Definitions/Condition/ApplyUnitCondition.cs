using Domain.Entities.Actions.ValueSources;
using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.Entities.Actions.Definitions.Condition;

public sealed class ApplyUnitCondition : IActionDefinition
{
    public UnitConditionDefinitionId UnitConditionDefinitionId { get; init; } = null!;
    public ValueSource Stacks { get; init; } = ValueSource.Fixed(1);
}