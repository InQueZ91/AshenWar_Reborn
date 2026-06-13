using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Domain.Events.Match;

public sealed record MatchEnded(MatchId MatchId, UserId? WinnerPlayerId) : IDomainEvent; // null if draw