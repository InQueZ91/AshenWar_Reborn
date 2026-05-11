using System.Collections.Generic;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects;

namespace Domain.Entities.Abilities.TargetShapes;

public class Cone(int range) : ITargetShape
{
    public bool RequiresInput => true;
    public int Range { get; } = range;

    public IEnumerable<ITargetable> Resolve(HexCoord origin, ITargetable input, IBoard board)
        => board.GetInCone(origin, input.Position, Range);
}