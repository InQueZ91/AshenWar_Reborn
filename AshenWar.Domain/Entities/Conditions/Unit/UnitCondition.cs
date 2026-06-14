using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Unit;

public sealed class UnitCondition : ConditionBase, IModifierSource
{
    public override UnitConditionDefinition Definition { get; }

    private UnitCondition(ConditionId id, UnitConditionDefinition definition, int remainingDuration, int stacks)
        : base(id, remainingDuration, stacks, definition.MaxStacks)
        => Definition = definition;
    
    public static UnitCondition Instantiate(UnitConditionDefinition definition, int stacks = 1)
    {
        ArgumentNullException.ThrowIfNull(definition);
        
        return new UnitCondition(ConditionId.New(), definition, definition.BaseDuration, stacks);
    }

    public static UnitCondition Rehydrate(ConditionId id,
        UnitConditionDefinition definition,
        int remainingDuration,
        int remainingStacks)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(definition);
        
        return new UnitCondition(id, definition, remainingDuration, remainingStacks);   
    }

    public IEnumerable<(Modifier Modifier, int Stacks)> GetModifiers() 
        => Definition.Modifiers.Select(m => (m, Stacks: CurrentStacks));

    public override void AddStacks(int stacks) => RebuildModifiers();
    public override void SetStacks(int stacks) => RebuildModifiers();

    // Modifiers depend on stacks - rebuild when stacks change
    // ModifierCalculator calls GetModifiers() fresh each time
    // so no cache needed - Build(Stacks) is called on demand
    private void RebuildModifiers() {} // no-op - GetModifiers() is called fresh each time
}