using System;
using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities.Unit.Events
{
    public sealed record UnitStaminaUpdated(UnitId UnitId, int CurrentStamina, int BaseStamina): IDomainEvent;
}