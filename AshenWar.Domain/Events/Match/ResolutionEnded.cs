using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Match;

namespace Domain.Events.Match;

public sealed record ResolutionEnded(TurnId TurnId) : IDomainEvent;