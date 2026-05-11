using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public sealed record UnitMoved(UnitId UnitId, HexCoord From, HexCoord To) : IDomainEvent;