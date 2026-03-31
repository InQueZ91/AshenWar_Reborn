using Domain.Enums;

namespace Domain.Interfaces.Modifier;

public interface IAbilityStatModifier : IModifier
{
    AbilityStat AbilityStat { get; }
}