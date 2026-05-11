using Domain.Interfaces;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.Identifiers.Match;

namespace Domain.Events.Match;

public sealed record TurnStarted(TurnId TurnId, int TurnNumber) : IDomainEvent;