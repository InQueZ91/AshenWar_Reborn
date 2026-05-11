using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events;

public sealed record AbilityCostPaid(UnitId UnitId, string ResourceType, int Amount) : IDomainEvent;