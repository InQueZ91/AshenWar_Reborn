using System.Numerics;
using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities.Unit.Events
{
    public sealed record UnitMoved(UnitId UnitId, Vector2 From, Vector2 To) : IDomainEvent;
}