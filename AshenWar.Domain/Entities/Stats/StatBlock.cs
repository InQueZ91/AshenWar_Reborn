using System.Collections.Generic;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Stats;

public sealed class StatBlock
{
    private readonly Dictionary<StatDefinitionId, float> _values;

    private StatBlock(Dictionary<StatDefinitionId, float> values)
    {
        _values = values;
    }
    
    public static StatBlock Empty => new (new Dictionary<StatDefinitionId, float>());

    public float Get(StatDefinition stat)
    {
        return _values.TryGetValue(stat.Id, out var value)
            ? value
            : stat.DefaultValue;
    }

    public StatBlock With(StatDefinition stat, float value)
    {
        var copy = new Dictionary<StatDefinitionId, float>(_values)
        {
            [stat.Id] = value
        };
        return new StatBlock(copy);
    }
}