using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Abilities;

namespace Domain.Entities.Actions.Definitions.Ability;

public sealed class RemovePassive : IActionDefinition
{
    public PassiveAbilityDefinitionId PassiveAbilityDefinitionId { get; init; } = null!;
}