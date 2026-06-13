using System.Collections.Generic;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Interfaces.Abilities;

public interface IHasAbilities
{
    IReadOnlyList<Ability> Abilities { get; }
    Ability? GetAbilityByDefinitionId(AbilityDefinitionId abilityDefinitionId);
    Ability? GetAbilityById(AbilityId abilityId);
}