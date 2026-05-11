using Domain.Entities.Conditions.Unit;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Conditions;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Interfaces.Entities;

public interface IUnit : ITargetable, IPlayerOwned, IStatHolder, IConditionHolder<UnitCondition>, IAbilityHolder
{
    UnitId Id { get; }
    bool IsAlive { get; }
    int CurrentHealth { get; }
    int CurrentStamina { get; }
    int CurrentSteps { get; }
}