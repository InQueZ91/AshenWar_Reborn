using System;

namespace Domain.ValueObjects.Identifiers;

public record VisualId(Guid Value) : Id<VisualId>(Value)
{
    public static VisualId New() => new(Guid.NewGuid());
}