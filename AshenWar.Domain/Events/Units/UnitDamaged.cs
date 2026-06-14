using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events.Units;

public sealed record UnitDamaged(UnitId UnitId, int Amount, int RemainingHealth) : IDomainEvent;