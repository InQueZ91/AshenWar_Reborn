using System;

namespace Domain.ValueObjects.Identifiers;

public abstract record Id<T>(Guid Value)
{
    public override string ToString() => Value.ToString();
}