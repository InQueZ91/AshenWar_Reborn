using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.TargetFilters.Primitives;

public sealed record EnemiesOnly : ITargetFilter
{
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
    {
        if (source is not IPlayerOwned ownedSource) return [];
        
        return candidates
            .OfType<IPlayerOwned>()
            .Where(t => t.Owner != ownedSource.Owner) // Enemies only
            .Cast<ITargetable>();
    }
}