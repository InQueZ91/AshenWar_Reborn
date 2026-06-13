using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Application.ValueObjects.Roll;

public sealed record UnitRoll(UnitId UnitId, int Speed, int Roll, int Total);