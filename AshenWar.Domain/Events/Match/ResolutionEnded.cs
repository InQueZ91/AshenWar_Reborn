using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Domain.Events.Match;

public sealed record ResolutionEnded(TurnId TurnId) : IDomainEvent;