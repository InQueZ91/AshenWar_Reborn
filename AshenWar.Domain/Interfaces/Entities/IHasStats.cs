using AshenWar.Domain.Entities.Stats;

namespace AshenWar.Domain.Interfaces.Entities;

public interface IHasStats
{
    StatBlock Stats { get; }
    int GetFinalStat(StatDefinition stat);
}