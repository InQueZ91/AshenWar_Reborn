using System;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Global;

public sealed class GlobalCondition : ConditionBase
{
    public override GlobalConditionDefinition Definition { get; }
    
    private GlobalCondition(ConditionId id, GlobalConditionDefinition definition, int remainingDuration, int stacks) 
        : base(id, remainingDuration, stacks, definition.MaxStacks)
        => Definition = definition;

    public static GlobalCondition Instantiate(GlobalConditionDefinition definition, int stacks = 1)
    {
        ArgumentNullException.ThrowIfNull(definition);
        
        return new GlobalCondition(ConditionId.New(), definition, definition.BaseDuration, stacks);
    }

    public static GlobalCondition Rehydrate(ConditionId id,
        GlobalConditionDefinition definition,
        int remainingDuration,
        int remainingStacks)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(definition);
        
        return new GlobalCondition(id, definition, remainingDuration, remainingStacks);
    }
}