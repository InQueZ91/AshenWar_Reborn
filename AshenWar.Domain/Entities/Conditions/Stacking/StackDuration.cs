using System;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Stacking;

/// Add incoming duration to remaining — incoming discarded
public sealed class StackDuration : IStackingBehavior
{
    public void Apply(ConditionBase existing, ConditionBase incoming, Action<ConditionBase> add, Action<ConditionBase> remove)
        => existing.AddDuration(incoming.Definition.BaseDuration);
}