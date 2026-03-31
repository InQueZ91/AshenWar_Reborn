using Domain.Enums;

namespace Domain.Interfaces.Modifier;

public interface IUnitStatModifier : IModifier
{
    UnitStat UnitStat { get; }
}