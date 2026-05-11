using System;

namespace Domain.ValueObjects.Identifiers.Units
{
    public record UnitId(Guid Value) : Id<UnitId>(Value)
    {
        public static UnitId New() => new(Guid.NewGuid());
    }
}