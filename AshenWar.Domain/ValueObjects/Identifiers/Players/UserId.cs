using System;

namespace AshenWar.Domain.ValueObjects.Identifiers.Players;

public record UserId(Guid Value) : Id<UserId>(Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static readonly UserId Neutral = new(Guid.Empty);
};