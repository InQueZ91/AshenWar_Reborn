using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Abilities.TargetFilters;

public class AnyOf(params ITargetFilter[] filters) : ITargetFilter
{
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
    {
        var list = candidates.ToList();
        return filters.SelectMany(f => f.Apply(list, source)).Distinct();
    }
}