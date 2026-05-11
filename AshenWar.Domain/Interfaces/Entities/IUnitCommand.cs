using Domain.Entities.Conditions.Unit;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Conditions;
using Domain.ValueObjects;

namespace Domain.Interfaces.Entities;

public interface IUnitCommand : IUnit, IConditionCommand<UnitCondition>, IAbilityCommand
{
    // Actions
    void TakeDamage(int amount);
    void RestoreHealth(int amount);

    void OperateStamina(int delta);
    void OperateSteps(int delta);
    
    void MoveTo(HexCoord destination);
}