using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events.Units;

public sealed record UnitHealed(UnitId UnitId, int Amount, int CurrentHealth) : IDomainEvent;