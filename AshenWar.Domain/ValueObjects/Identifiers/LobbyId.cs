using System;

namespace AshenWar.Domain.ValueObjects.Identifiers;

public sealed record LobbyId(Guid Value) : Id<LobbyId>(Value)
{
    public static LobbyId New() => new(Guid.NewGuid());
}