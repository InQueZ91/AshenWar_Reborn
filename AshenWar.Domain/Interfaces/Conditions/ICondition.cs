using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Interfaces.Conditions;

public interface ICondition<TCondition> : IHasConditions<TCondition>
    where TCondition : ConditionBase
{
    /// Route through stacking logic - public entry point for the application layer
    void ApplyCondition(TCondition condition);
    
    /// Remove by instance id - used by ConditionExecutor on expiry or dispel
    void RemoveCondition(ConditionId conditionId);
}