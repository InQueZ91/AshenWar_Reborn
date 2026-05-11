using System.Collections.Generic;
using System.Linq;
using Domain.Entities.Modifiers;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Conditions.Tile;

public sealed class TileCondition : ConditionBase, IModifierProvider
{
    public override TileConditionDefinition Definition { get; }

    private TileCondition(TileConditionDefinition definition, int stacks) :
        base(definition.BaseDuration, stacks, definition.MaxStacks)
        => Definition = definition;

    public static TileCondition Instantiate(TileConditionDefinition definition, int stacks = 1)
        => new(definition, stacks);
    
    public IEnumerable<(ModifierDefinition Definition, int Stacks)> GetModifierEntries() 
        => Definition.ModifierDefinitions.Select(m => (m, Stacks));

    public override void AddStacks(int stacks) => RebuildModifiers();
    public override void SetStacks(int stacks) => RebuildModifiers();

    // Modifiers depend on stacks - rebuild when stacks change
    // ModifierCalculator calls GetModifiers() fresh each time
    // so no cache needed - Build(Stacks) is called on demand
    private void RebuildModifiers() {} // no-op - GetModifiers() is called fresh each time
}