using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Interfaces.Entities;

public interface ITargetable
{
    HexCoord? Position { get; }
}