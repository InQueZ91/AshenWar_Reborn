using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;

namespace Application.Events;

public sealed record MatchEnded(MatchId MatchId, UserId? WinnerId);