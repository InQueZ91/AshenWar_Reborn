using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Abilities.TargetFilters;

public class AllOf(params ITargetFilter[] filters) : ITargetFilter
{
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
    {
        return filters.Aggregate(candidates, (current, filter) => filter.Apply(current, source));
    }
}