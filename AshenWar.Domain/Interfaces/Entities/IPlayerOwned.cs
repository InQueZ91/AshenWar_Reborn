using Domain.ValueObjects.Identifiers.Players;

namespace Domain.Interfaces.Entities;

public interface IPlayerOwned
{
    UserId Owner { get; }
}