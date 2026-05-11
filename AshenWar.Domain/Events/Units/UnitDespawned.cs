using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public record UnitDespawned(UnitId UnitId) : IDomainEvent;