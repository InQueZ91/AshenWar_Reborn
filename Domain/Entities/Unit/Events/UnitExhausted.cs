using System;
using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities.Unit.Events
{
    public sealed record UnitExhausted(UnitId UnitId) : IDomainEvent;
}