using Domain.Abstractions;

namespace Domain.Entities.Unit.Events
{
    public sealed record UnitCreated(Unit CreatedUnit) : IDomainEvent;
}