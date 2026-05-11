using Domain.Entities.Stats;

namespace Domain.Interfaces.Entities;

public interface IStatHolder 
{
    int GetMaxStat(StatDefinition stat);
}