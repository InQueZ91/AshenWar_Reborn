using Application.Interfaces;
using Domain.ValueObjects.Identifiers.Match;

namespace Application.Events;

public record TurnPlanningExpired(TurnId TurnId, MatchId MatchId);