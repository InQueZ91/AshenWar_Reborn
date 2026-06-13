using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Domain.Entities.Actions.Definitions.Ability.Cooldown;

public sealed record SetAbilityCooldown(AbilityDefinitionId AbilityDefinitionId, int Duration) : IActionDefinition;