using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.TargetFilters.Primitives;

public sealed record TilesOnly : ITargetFilter
{
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
        => candidates.OfType<IReadOnlyTile>();
}