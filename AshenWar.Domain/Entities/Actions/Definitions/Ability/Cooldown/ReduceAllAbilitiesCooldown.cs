using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Actions.Definitions.Ability.Cooldown;

public sealed record ReduceAllAbilitiesCooldown(int Amount) : IActionDefinition;