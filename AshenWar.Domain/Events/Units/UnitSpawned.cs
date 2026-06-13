using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events.Units;

public record UnitSpawned(UnitId UnitId, HexCoord Position) : IDomainEvent;