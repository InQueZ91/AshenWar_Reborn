using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.Entities.Actions.Definitions.Condition;

public sealed class RemoveUnitCondition : IActionDefinition
{
    public UnitConditionDefinitionId UnitConditionDefinitionId { get; init; } = null!;
}