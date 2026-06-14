using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.TargetFilters;

public sealed record AllOf : ITargetFilter
{
    public required IReadOnlyList<ITargetFilter> Filters { get; init; }
    
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
    {
        return Filters.Aggregate(candidates, (current, filter) => filter.Apply(current, source));
    }
}