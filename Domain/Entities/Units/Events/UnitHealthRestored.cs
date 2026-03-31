using Domain.Interfaces;

namespace Domain.Entities.Units.Events;

public sealed record UnitHealthRestored() : IDomainEvent;