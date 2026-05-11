using System;
using Domain.Interfaces.Conditions;

namespace Domain.Entities.Conditions.Stacking;

/// Reset existing duration to base — incoming discarded
public sealed class RefreshDuration : IStackingBehavior
{
    public void Apply(ConditionBase existing, ConditionBase incoming, Action<ConditionBase> add, Action<ConditionBase> remove)
        => existing.RefreshDuration();
}