using System.Collections.Generic;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Abilities.TargetShapes;

public sealed record Self : ITargetShape
{
    public bool RequiresInput => false;
    public IEnumerable<ITargetable> Resolve(HexCoord origin, ITargetable input, IReadOnlyBoard readOnlyBoard)
        => readOnlyBoard.GetAt(origin);
}