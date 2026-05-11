using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Abilities.TargetFilters.Primitives;

public class EmptyTiles : ITargetFilter
{
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source)
        => candidates.OfType<ITile>().Where(t => !t.IsOccupied);
}