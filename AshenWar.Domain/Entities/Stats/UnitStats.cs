using System.Collections.Generic;

namespace AshenWar.Domain.Entities.Stats;

public sealed class UnitStats(Dictionary<StatDefinition, int> values) : StatBlock(values, ValidStats, Defaults)
{
    private static readonly IReadOnlySet<StatDefinition> ValidStats = new HashSet<StatDefinition>
    {
        StatDefinition.Power,
        StatDefinition.Health,
        StatDefinition.Stamina,
        StatDefinition.Steps,
        StatDefinition.Speed,
        StatDefinition.Vision,
    };

    private static readonly Dictionary<StatDefinition, int> Defaults = new()
    {
        [StatDefinition.Power] = 1,
        [StatDefinition.Health] = 0,
        [StatDefinition.Stamina] = 0,
        [StatDefinition.Steps] = 0,
        [StatDefinition.Speed] = 1,
        [StatDefinition.Vision] = 1,
    };
}