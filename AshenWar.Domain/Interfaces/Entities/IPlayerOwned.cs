using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Domain.Interfaces.Entities;

public interface IPlayerOwned
{
    UserId Owner { get; }
}