using Domain.Interfaces;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events.Units;

public sealed record UnitStepsChanged(UnitId UnitId, int Delta, int CurrentSteps) : IDomainEvent;