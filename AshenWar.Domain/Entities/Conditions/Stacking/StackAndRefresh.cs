using System;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Stacking;

public sealed class StackAndRefresh : IStackingBehavior
{
    public void Apply(ConditionBase existing, ConditionBase incoming, Action<ConditionBase> add, Action<ConditionBase> remove)
    {
        existing.AddStacks(incoming.CurrentStacks);
        existing.RefreshDuration();
    }
}