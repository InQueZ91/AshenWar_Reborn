using System;

namespace Domain.ValueObjects.Identifiers;

public abstract record Id<T>(Guid Value) where T : Id<T>
{
    public override string ToString() => Value.ToString();
}