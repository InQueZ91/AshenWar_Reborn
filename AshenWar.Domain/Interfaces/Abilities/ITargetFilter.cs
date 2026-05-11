using System.Collections.Generic;
using Domain.Interfaces.Entities;

namespace Domain.Interfaces.Abilities;

public interface ITargetFilter
{
    IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source);
}