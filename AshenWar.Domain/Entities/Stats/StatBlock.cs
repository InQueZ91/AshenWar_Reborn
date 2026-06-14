using System;
using System.Collections.Generic;
using System.Linq;

namespace AshenWar.Domain.Entities.Stats;

public abstract class StatBlock
{
    private readonly Dictionary<StatDefinition, int> _values;
    private readonly IReadOnlySet<StatDefinition> _validStats;
    private readonly Dictionary<StatDefinition, int> _defaults;
    
    protected StatBlock(
        Dictionary<StatDefinition, int> values,
        IReadOnlySet<StatDefinition> validStats,
        Dictionary<StatDefinition, int> defaults)
    {
        var invalid = values.Keys.Except(validStats).ToList();
        if (invalid.Count != 0)
            throw new ArgumentException(
                $"Invalid stats: {string.Join(", ", invalid.Select(s => s.Name))}");

        _values = values;
        _validStats = validStats;
        _defaults = defaults;
    }

    private int GetDefault(StatDefinition stat) => _defaults.GetValueOrDefault(stat, 0);

    public int Get(StatDefinition stat)
    {
        if (!_validStats.Contains(stat))
            throw new ArgumentException($"Stat '{stat.Name}' is not valid for {GetType().Name}");

        return _values.TryGetValue(stat, out var value) ? value : GetDefault(stat);
    }
}