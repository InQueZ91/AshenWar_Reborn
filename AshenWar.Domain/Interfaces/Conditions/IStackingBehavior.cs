using System;
using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Entities.Conditions.Stacking;

namespace AshenWar.Domain.Interfaces.Conditions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(NoneStacking), "None")]
[JsonDerivedType(typeof(RefreshDuration), "RefreshDuration")]
[JsonDerivedType(typeof(ReplaceStacking), "Replace")]
[JsonDerivedType(typeof(StackAndRefresh), "StackAndRefresh")]
[JsonDerivedType(typeof(StackDuration), "StackDuration")]
[JsonDerivedType(typeof(StackIntensity), "StackIntensity")]
public interface IStackingBehavior
{
    void Apply(
        ConditionBase existing,
        ConditionBase incoming,
        Action<ConditionBase> add,
        Action<ConditionBase> remove);
}