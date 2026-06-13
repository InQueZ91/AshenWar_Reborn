using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events.Units;

public sealed record UnitStaminaChanged(UnitId  UnitId, int Delta, int CurrentStamina) : IDomainEvent;