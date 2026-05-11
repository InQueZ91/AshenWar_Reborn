using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Abilities.TargetFilters.Primitives;

public class EnemiesOnly : ITargetFilter
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