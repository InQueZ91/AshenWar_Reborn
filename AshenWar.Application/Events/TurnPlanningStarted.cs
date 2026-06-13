using System;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Application.Events;

public record TurnPlanningStarted(
    MatchId MatchId,
    TurnId TurnId,
    DateTimeOffset StartedAt,
    TimeSpan Duration);