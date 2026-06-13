using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Application.Events;

public record TurnPlanningExpired(TurnId TurnId, MatchId MatchId);