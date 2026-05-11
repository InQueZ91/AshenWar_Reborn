using System;
using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Match;

namespace Domain.Events.Match;

public sealed record PlanningStarted(TurnId TurnId, int TurnNumber, DateTimeOffset StartedAt, TimeSpan Duration) : IDomainEvent;