using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Domain.Entities.Actions.Definitions.Ability;

public sealed record GrantActive(AbilityDefinitionId AbilityDefinitionId) : IActionDefinition; 