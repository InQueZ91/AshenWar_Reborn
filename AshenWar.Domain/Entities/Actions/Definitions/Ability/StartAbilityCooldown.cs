using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Abilities;

namespace Domain.Entities.Actions.Definitions.Ability;

public sealed class StartAbilityCooldown : IActionDefinition
{
    public ActiveAbilityDefinitionId AbilityDefinitionId { get; init; } = null!;
}