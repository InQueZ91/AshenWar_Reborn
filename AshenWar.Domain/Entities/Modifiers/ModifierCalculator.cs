using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Modifiers;

public static class ModifierCalculator
{
    public static int Calculate(
        float baseValue,
        IEnumerable<IModifierProvider> providers,
        StatDefinitionId statId, 
        IReadOnlySet<EntityTag> entityTags)
    {
        var entries = providers
            .SelectMany(p => p.GetModifierEntries())
            .Where(e => e.Definition.StatId == statId)
            .Where(e => e.Definition.ScopeFilter.Count == 0
                        || e.Definition.ScopeFilter.IsSubsetOf(entityTags))
            .OrderBy(e => e.Definition.Operation.Phase)
            .ToList();

        var result = baseValue;
        foreach (var entry in entries)
        {
            var value = entry.Definition.CalculateValue(entry.Stacks);
            result = entry.Definition.Operation.Apply(result, value);
        }
        
        return (int)Math.Round(result);
    }
}