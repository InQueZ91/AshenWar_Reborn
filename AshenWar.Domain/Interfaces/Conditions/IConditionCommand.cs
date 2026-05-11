using Domain.Entities.Conditions;
using Domain.ValueObjects.LocalIdentifiers;
using Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace Domain.Interfaces.Conditions;

public interface IConditionCommand<TCondition> : IConditionHolder<TCondition>
    where TCondition : ConditionBase
{
    /// Route through stacking logic - public entry point for the application layer
    void ApplyCondition(TCondition condition);
    
    /// Remove by instance id - used by ConditionExecutor on expiry or dispel
    void RemoveCondition(ConditionId conditionId);
}