using System;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Stacking;

/// Ignore incoming — existing condition unchanged
public sealed class NoneStacking : IStackingBehavior
{
    public void Apply(ConditionBase existing,
        ConditionBase incoming,
        Action<ConditionBase> add,
        Action<ConditionBase> remove)
    {
        // Do nothing
    }
}