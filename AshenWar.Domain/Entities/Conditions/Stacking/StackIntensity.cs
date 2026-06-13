using System;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Stacking;

/// Add stacks to existing — increases modifier intensity
public sealed class StackIntensity : IStackingBehavior
{
    public void Apply(ConditionBase existing, ConditionBase incoming, Action<ConditionBase> add, Action<ConditionBase> remove)
        => existing.AddStacks(incoming.CurrentStacks);
}