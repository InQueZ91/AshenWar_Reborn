using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Tile;

public sealed class TileCondition : ConditionBase, IModifierSource
{
    public override TileConditionDefinition Definition { get; }

    private TileCondition(ConditionId id, TileConditionDefinition definition, int remainingDuration, int stacks) :
        base(id, remainingDuration, stacks, definition.MaxStacks)
        => Definition = definition;

    public static TileCondition Instantiate(TileConditionDefinition definition, int stacks = 1)
    {
        ArgumentNullException.ThrowIfNull(definition);
        
        return new TileCondition(ConditionId.New(), definition, definition.BaseDuration, stacks);
    }

    public static TileCondition Rehydrate(ConditionId id,
        TileConditionDefinition definition,
        int remainingDuration,
        int remainingStacks)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(definition);
        
        return new TileCondition(id, definition, remainingDuration, remainingStacks);
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