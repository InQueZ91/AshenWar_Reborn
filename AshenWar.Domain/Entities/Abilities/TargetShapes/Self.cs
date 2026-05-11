using System.Collections.Generic;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects;

namespace Domain.Entities.Abilities.TargetShapes;

public class Self : ITargetShape
{
    public bool RequiresInput => false;
    public IEnumerable<ITargetable> Resolve(HexCoord origin, ITargetable input, IBoard board)
        => board.GetAt(origin);
}