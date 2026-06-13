using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Application.Events;

public sealed record MatchEnded(MatchId MatchId, UserId WinnerId);