using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Modifiers;

public static class ModifierCalculator
{
    public static int Calculate(
        float baseValue,
        StatDefinition stat,
        IEnumerable<IModifierSource> modifierSources,
        IReadOnlySet<EntityTag> entityTags)
    {
        var entries = modifierSources
            .SelectMany(p => p.GetModifiers())
            .Where(e => e.Modifier.Stat == stat)
            .Where(e => e.Modifier.TagFilter.Count == 0
                        || e.Modifier.TagFilter.IsSubsetOf(entityTags))
            .OrderBy(e => e.Modifier.Operation.Phase)
            .ToList();

        var result = baseValue;
        foreach (var entry in entries)
        {
            var value = entry.Modifier.CalculateValue(entry.Stacks);
            result = entry.Modifier.Operation.Apply(result, value);
        }
        
        return (int)Math.Round(result);
    }
}