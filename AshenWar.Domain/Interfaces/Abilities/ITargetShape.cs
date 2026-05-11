using System.Collections.Generic;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects;

namespace Domain.Interfaces.Abilities;

public interface ITargetShape
{
    bool RequiresInput { get; }
    IEnumerable<ITargetable> Resolve(HexCoord origin, ITargetable input, IBoard board);
}