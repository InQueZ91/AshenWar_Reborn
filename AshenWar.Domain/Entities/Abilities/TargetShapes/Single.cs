using System.Collections.Generic;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects;

namespace Domain.Entities.Abilities.TargetShapes;

public class Single : ITargetShape
{
    public bool RequiresInput => true;
    public IEnumerable<ITargetable> Resolve(HexCoord origin, ITargetable input, IBoard board) 
        => [input];
}