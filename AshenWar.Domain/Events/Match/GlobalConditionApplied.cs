using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Domain.Events.Match;

public record GlobalConditionApplied(GlobalConditionDefinitionId GlobalConditionDefinitionId) : IDomainEvent;