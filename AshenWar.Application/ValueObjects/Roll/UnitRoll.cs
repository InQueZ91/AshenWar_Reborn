using Domain.ValueObjects.Identifiers.Units;

namespace Application.ValueObjects.Roll;

public sealed record UnitRoll(UnitId UnitId, int Speed, int Roll, int Total);