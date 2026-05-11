using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.Events.Match;

public record GlobalConditionApplied(GlobalConditionDefinitionId GlobalConditionDefinitionId) : IDomainEvent;