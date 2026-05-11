using Domain.Entities.Abilities.Passive;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Interfaces.Abilities.Passive;

public interface IPassiveAbilityCommand : IPassiveAbilityHolder
{
    void AddPassive(PassiveAbility ability);
    void RemovePassive(PassiveAbilityId passiveAbilityId);
}