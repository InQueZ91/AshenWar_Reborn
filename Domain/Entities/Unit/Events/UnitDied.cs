using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities.Unit.Events
{
    public sealed record UnitDied(UnitId UnitId) : IDomainEvent;
}