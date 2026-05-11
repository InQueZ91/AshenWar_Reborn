using System;

namespace Domain.ValueObjects.Identifiers.Match;

public sealed record MatchDefinitionId(Guid Value) : Id<MatchDefinitionId>(Value)
{
    public static MatchDefinitionId New() => new(Guid.NewGuid());
}