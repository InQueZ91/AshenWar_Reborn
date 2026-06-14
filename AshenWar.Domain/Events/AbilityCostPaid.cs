using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events;

public sealed record AbilityCostPaid(UnitId UnitId, string ResourceType, int Amount) : IDomainEvent;