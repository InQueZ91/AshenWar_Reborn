using System;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Entities.Conditions;

public abstract class ConditionBase
{
    // Static
    public ConditionId Id { get; private set;}
    public abstract ConditionDefinitionBase Definition { get; }
    
    // Runtime
    public int RemainingDuration { get; private set;}
    public int CurrentStacks { get; private set; }
    public bool IsExpired => RemainingDuration <= 0;
    
    protected ConditionBase(ConditionId id, int remainingDuration, int stacks, int maxStacks) 
    {
        Id = id;
        RemainingDuration = remainingDuration;
        CurrentStacks = Math.Clamp(stacks, 1, maxStacks);
    }

    public void Tick() {if (RemainingDuration > 0) RemainingDuration--;}
    public void RefreshDuration() => RemainingDuration = Definition.BaseDuration;
    public void AddDuration(int duration) => RemainingDuration += duration;
    
    public virtual void AddStacks(int stacks) 
        => CurrentStacks = Math.Min(CurrentStacks + stacks, Definition.MaxStacks);
    public virtual void SetStacks(int stacks)
        => CurrentStacks = Math.Clamp(stacks, 1, Definition.MaxStacks);
}