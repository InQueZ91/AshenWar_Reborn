using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Match;

public record TurnId(Guid Value) : Id<TurnId>(Value)
{
    public static TurnId New() => new(Guid.NewGuid());
}