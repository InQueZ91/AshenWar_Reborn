using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public record UnitSpawned(UnitId UnitId, HexCoord Position) : IDomainEvent;