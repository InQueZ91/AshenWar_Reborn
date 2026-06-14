using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Interfaces.Entities;

public interface IUnit : 
    IReadOnlyUnit,
    IAbility,
    IPassive,
    ICondition<UnitCondition>
{
    // Actions
    void TakeDamage(int amount);
    void RestoreHealth(int amount);

    void OperateStamina(int delta);
    void OperateSteps(int delta);
    
    void MoveTo(HexCoord destination);
}