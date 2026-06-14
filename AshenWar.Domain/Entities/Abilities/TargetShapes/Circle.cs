using System.Collections.Generic;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Abilities.TargetShapes;

public sealed record Circle(int Radius) : ITargetShape
{
    public bool RequiresInput => true;
    public IEnumerable<ITargetable> Resolve(HexCoord origin, ITargetable input, IReadOnlyBoard readOnlyBoard)
    {
        if (input.Position is null)
            throw new DomainException($"No input position for {input} to resolve circle");
        
        return readOnlyBoard.GetInRange(input.Position, Radius);
    }
}