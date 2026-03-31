using Domain.Enums;

namespace Domain.Interfaces.Modifier;

public interface IDamageModifier : IModifier
{
    DamageType DamageType { get; }
}