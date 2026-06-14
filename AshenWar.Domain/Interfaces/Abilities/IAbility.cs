using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Interfaces.Abilities;

public interface IAbility : IHasAbilities
{
    void AddAbility(Ability ability);
    void RemoveAbility(AbilityId abilityId);
}