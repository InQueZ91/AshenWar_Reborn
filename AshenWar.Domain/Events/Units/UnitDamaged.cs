using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public sealed record UnitDamaged(UnitId UnitId, int Amount, int RemainingHealth) : IDomainEvent;