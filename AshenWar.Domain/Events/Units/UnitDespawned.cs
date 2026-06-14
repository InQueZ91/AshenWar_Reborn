using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events.Units;

public record UnitDespawned(UnitId UnitId) : IDomainEvent;