using Domain.Entities.Stats;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Actions.ValueSources;

public sealed class FromStatValueSource(StatDefinition stat) : ValueSource
{
    public override float Resolve(ActionContext context)
    {
        if (context.Source is IStatHolder holder)
            return holder.GetMaxStat(stat);
        
        return 0f;
    }
}