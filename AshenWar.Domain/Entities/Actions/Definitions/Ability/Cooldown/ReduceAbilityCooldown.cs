using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Domain.Entities.Actions.Definitions.Ability.Cooldown;

public sealed record ReduceAbilityCooldown(AbilityDefinitionId AbilityDefinitionId, int Amount) : IActionDefinition;