using System;
using Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace Domain.Entities.Conditions;

public abstract class ConditionBase(int baseDuration, int stacks, int maxStacks)
{
    // Static
    public ConditionId Id { get; } = ConditionId.New();
    public abstract ConditionDefinitionBase Definition { get; }
    
    // Runtime
    public int RemainingDuration { get; private set;} = baseDuration;
    public int Stacks { get; private set;} = Math.Clamp(stacks, 1, maxStacks);
    public bool IsExpired => RemainingDuration <= 0;

    public void Tick() {if (RemainingDuration > 0) RemainingDuration--;}
    public void RefreshDuration() => RemainingDuration = Definition.BaseDuration;
    public void AddDuration(int duration) => RemainingDuration += duration;
    
    public virtual void AddStacks(int stacks) 
        => Stacks = Math.Min(Stacks + stacks, Definition.MaxStacks);
    public virtual void SetStacks(int stacks)
        => Stacks = Math.Clamp(stacks, 1, Definition.MaxStacks);
}