using System;
using Domain.Interfaces.Conditions;

namespace Domain.Entities.Conditions.Stacking;

/// Add stacks to existing — increases modifier intensity
public sealed class StackIntensity : IStackingBehavior
{
    public void Apply(ConditionBase existing, ConditionBase incoming, Action<ConditionBase> add, Action<ConditionBase> remove)
        => existing.AddStacks(incoming.Stacks);
}