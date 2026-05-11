using Domain.Interfaces.Abilities.Active;
using Domain.Interfaces.Abilities.Passive;

namespace Domain.Interfaces.Abilities;

public interface IAbilityCommand : IAbilityHolder, IActiveAbilityCommand, IPassiveAbilityCommand
{
}