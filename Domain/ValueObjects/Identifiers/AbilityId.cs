using System;

namespace Domain.ValueObjects.Identifiers;

public record AbilityId(Guid Value) : Id<AbilityId>(Value)
{
    public static AbilityId New() => new(Guid.NewGuid());
}