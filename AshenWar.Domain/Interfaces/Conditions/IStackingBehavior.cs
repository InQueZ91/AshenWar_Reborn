using System;
using Domain.Entities.Conditions;

namespace Domain.Interfaces.Conditions;

public interface IStackingBehavior
{
    void Apply(
        ConditionBase existing,
        ConditionBase incoming,
        Action<ConditionBase> add,
        Action<ConditionBase> remove);
}