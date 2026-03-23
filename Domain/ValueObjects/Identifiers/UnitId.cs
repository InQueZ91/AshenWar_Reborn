using System;

namespace Domain.ValueObjects.Identifiers
{
    public readonly record struct UnitId(Guid Value)
    {
        public static UnitId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}