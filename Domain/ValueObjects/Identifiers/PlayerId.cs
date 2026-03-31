using System;

namespace Domain.ValueObjects.Identifiers;

public record PlayerId(Guid Value) : Id<PlayerId>(Value)
{
    public static PlayerId New() => new(Guid.NewGuid());
};