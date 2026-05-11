using System.Collections.Generic;
using Domain.Entities.Abilities.Active;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Interfaces.Abilities.Active;

public interface IActiveAbilityHolder
{
    IReadOnlyList<ActiveAbility> ActiveAbilities { get; }
    ActiveAbility? GetActiveAbilityByDefinitionId(ActiveAbilityDefinitionId abilityDefinitionId);
    ActiveAbility? GetActiveAbilityById(ActiveAbilityId abilityId);
}