using System.Collections.Generic;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.TargetFilters.Primitives;

public sealed record Self : ITargetFilter
{
    public IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source) 
        => new List<ITargetable> { source };
}