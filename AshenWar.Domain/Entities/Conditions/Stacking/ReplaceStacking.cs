using System;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Stacking;

/// Remove existing, apply fresh instance
public sealed class ReplaceStacking : IStackingBehavior
{
    public void Apply(ConditionBase existing, ConditionBase incoming, Action<ConditionBase> add, Action<ConditionBase> remove)
    {
        remove(existing);
        add(incoming);
    }
}