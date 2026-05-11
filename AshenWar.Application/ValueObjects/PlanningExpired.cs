using Domain.ValueObjects.Identifiers.Match;

namespace Application.ValueObjects;

public sealed record PlanningExpired(TurnId TurnId, MatchId MatchId);