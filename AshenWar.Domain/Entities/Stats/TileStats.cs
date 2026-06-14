using System.Collections.Generic;

namespace AshenWar.Domain.Entities.Stats;

public sealed class TileStats(Dictionary<StatDefinition, int> stats) : StatBlock(stats, ValidStats, Defaults)
{
    private static readonly IReadOnlySet<StatDefinition> ValidStats = new HashSet<StatDefinition>
    {
        StatDefinition.MovementCost,
    };

    private static readonly Dictionary<StatDefinition, int> Defaults = new()
    {
        [StatDefinition.MovementCost] = 1,
    };
}