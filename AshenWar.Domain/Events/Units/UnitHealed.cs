using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public sealed record UnitHealed(UnitId UnitId, int Amount, int CurrentHealth) : IDomainEvent;