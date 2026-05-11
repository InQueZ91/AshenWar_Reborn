using System.Collections.Generic;
using Domain.Entities.Abilities.Passive;
using Domain.ValueObjects.Identifiers.Abilities;

namespace Domain.Interfaces.Abilities.Passive;

public interface IPassiveAbilityHolder
{
    IReadOnlyList<PassiveAbility> PassiveAbilities { get; }
    PassiveAbility? GetPassiveByDefinitionId(PassiveAbilityDefinitionId abilityDefinitionId);
}