using System.Collections.Generic;

namespace AshenWar.Domain.Entities.Stats;

public sealed class AbilityStats(Dictionary<StatDefinition, int> stats) : StatBlock(stats, ValidStats, Defaults)
{
    private static readonly IReadOnlySet<StatDefinition> ValidStats = new HashSet<StatDefinition>
    {
        StatDefinition.Damage,
        StatDefinition.Cooldown,
        StatDefinition.Range,
    };

    private static readonly Dictionary<StatDefinition, int> Defaults = new()
    {
        [StatDefinition.Damage] = 0,
        [StatDefinition.Cooldown] = 0,
        [StatDefinition.Range] = 1,
    };
}