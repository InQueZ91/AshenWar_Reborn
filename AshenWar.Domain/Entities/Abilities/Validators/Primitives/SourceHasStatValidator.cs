using Domain.Entities.Actions;
using Domain.Entities.Stats;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Abilities.Validators.Primitives;

public sealed class SourceHasStatValidator(int minimum, StatDefinition stat) : IValidator
{
    public bool Check(ActionContext context) 
        => context.Source is IStatHolder holder && holder.GetMaxStat(stat) >= minimum;
}