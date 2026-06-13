using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.TargetFilters;

public class AnyOf : ITargetFilter
{
    public required IReadOnlyList<ITargetFilter> Filters { get; init; }
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
    {
        var list = candidates.ToList();
        return Filters.SelectMany(f => f.Apply(list, source)).Distinct();
    }
}