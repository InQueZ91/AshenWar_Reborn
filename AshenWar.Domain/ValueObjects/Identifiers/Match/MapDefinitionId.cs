using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Match;

public sealed record MapDefinitionId(Guid Value) : Id<MapDefinitionId>(Value)
{
    public static MapDefinitionId New() => new(Guid.NewGuid());
}