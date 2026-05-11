using Domain.Entities.Actions.ValueSources;
using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Abilities;

namespace Domain.Entities.Actions.Definitions.Ability;

public sealed class SetCooldown : IActionDefinition
{
    public ActiveAbilityDefinitionId AbilityDefinitionId { get; init; } = null!;
    public ValueSource Duration { get; init; } = null!;
}