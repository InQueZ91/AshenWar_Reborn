using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public sealed record UnitDied(UnitId UnitId, HexCoord Position) : IDomainEvent;