using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Abilities.TargetFilters;

public class NoneOf(params ITargetFilter[] filters) : ITargetFilter
{
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
    {
        var exclude = filters
            .SelectMany(f => f.Apply(candidates, source))
            .ToHashSet();
        return candidates.Where(c => !exclude.Contains(c));
    }
}