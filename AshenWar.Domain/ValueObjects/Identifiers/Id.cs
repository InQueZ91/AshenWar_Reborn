using System;

namespace AshenWar.Domain.ValueObjects.Identifiers;

public abstract record Id<T>(Guid Value) where T : Id<T>
{
    public sealed override string ToString() => Value.ToString();
}