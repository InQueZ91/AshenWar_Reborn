using Domain.Interfaces.Abilities.Active;
using Domain.Interfaces.Abilities.Passive;

namespace Domain.Interfaces.Abilities;

public interface IAbilityHolder : IActiveAbilityHolder, IPassiveAbilityHolder
{
    
}