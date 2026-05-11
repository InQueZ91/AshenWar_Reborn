using Domain.ValueObjects;

namespace Domain.Interfaces.Entities;

public interface ITargetable
{
    HexCoord? Position { get; }
}