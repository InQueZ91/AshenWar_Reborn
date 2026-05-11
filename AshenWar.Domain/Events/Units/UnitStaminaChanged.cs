using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public sealed record UnitStaminaChanged(UnitId  UnitId, int Delta, int CurrentStamina) : IDomainEvent;