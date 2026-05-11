using Domain.Interfaces;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;

namespace Domain.Events.Match;

public sealed record MatchEnded(MatchId MatchId, UserId? WinnerPlayerId) : IDomainEvent; // null if draw