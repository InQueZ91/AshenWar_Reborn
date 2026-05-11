using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.Entities.Actions.Definitions.Condition;

public sealed class RemoveGlobalCondition : IActionDefinition
{
    public GlobalConditionDefinitionId GlobalConditionDefinitionId { get; init; } = null!;
}