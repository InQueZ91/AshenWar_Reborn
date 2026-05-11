using System;
using Domain.Interfaces.Conditions;

namespace Domain.Entities.Conditions.Stacking;

public sealed class StackAndRefresh : IStackingBehavior
{
    public void Apply(ConditionBase existing, ConditionBase incoming, Action<ConditionBase> add, Action<ConditionBase> remove)
    {
        existing.AddStacks(incoming.Stacks);
        existing.RefreshDuration();
    }
}