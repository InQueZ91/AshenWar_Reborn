using Domain.Entities.Abilities.Active;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Interfaces.Abilities.Active;

public interface IActiveAbilityCommand : IActiveAbilityHolder
{
    void AddActiveAbility(ActiveAbility ability);
    void RemoveActiveAbility(ActiveAbilityId activeAbilityId);
}