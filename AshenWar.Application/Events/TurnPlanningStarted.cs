using System;
using Application.Interfaces;
using Domain.ValueObjects.Identifiers.Match;

namespace Application.Events;

public record TurnPlanningStarted(
    MatchId MatchId,
    TurnId TurnId,
    DateTimeOffset StartedAt,
    TimeSpan Duration);