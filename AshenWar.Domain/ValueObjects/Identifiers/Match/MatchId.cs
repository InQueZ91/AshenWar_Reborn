using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Match;

public record MatchId(Guid Value) : Id<MatchId>(Value)
{
    public static MatchId New() => new(Guid.NewGuid());
}