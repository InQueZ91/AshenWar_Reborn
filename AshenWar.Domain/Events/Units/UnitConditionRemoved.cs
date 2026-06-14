using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events.Units;

public sealed record UnitConditionRemoved(UnitId UnitId, UnitConditionDefinitionId UnitConditionDefinitionId) : IDomainEvent;