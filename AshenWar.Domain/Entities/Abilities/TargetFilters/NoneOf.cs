using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.TargetFilters;

public sealed record NoneOf : ITargetFilter
{
    public required IReadOnlyList<ITargetFilter> Filters { get; init; }
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
    {
        var exclude = Filters
            .SelectMany(f => f.Apply(candidates, source))
            .ToHashSet();
        return candidates.Where(c => !exclude.Contains(c));
    }
}