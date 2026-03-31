using Domain.Enums;

namespace Domain.Interfaces.Modifier;

public interface IModifier
{
    float Modifier { get; }
    ModifierType ModifierType { get; }
}