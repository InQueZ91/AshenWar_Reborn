using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Conditions;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public sealed record UnitConditionRemoved(UnitId UnitId, UnitConditionDefinitionId UnitConditionDefinitionId) : IDomainEvent;