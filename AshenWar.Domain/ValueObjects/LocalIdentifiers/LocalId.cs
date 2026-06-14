using System;

namespace AshenWar.Domain.ValueObjects.LocalIdentifiers;

public abstract record LocalId<T>(Guid Value) where T : LocalId<T>
{
    public static T New() => (T)Activator.CreateInstance(typeof(T), Guid.NewGuid())!;
}