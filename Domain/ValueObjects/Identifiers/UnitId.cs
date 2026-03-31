using System;

namespace Domain.ValueObjects.Identifiers
{
    public record UnitId(Guid Value) : Id<UnitId>(Value)
    {
        public static UnitId New() => new(Guid.NewGuid());
    }
}