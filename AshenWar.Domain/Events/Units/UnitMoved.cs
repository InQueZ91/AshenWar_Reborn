using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events.Units;

public sealed record UnitMoved(UnitId UnitId, HexCoord From, HexCoord To) : IDomainEvent;