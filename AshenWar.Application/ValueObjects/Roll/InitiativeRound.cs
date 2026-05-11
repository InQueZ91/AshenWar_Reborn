
using System.Collections.Generic;

namespace Application.ValueObjects.Roll;

public sealed record InitiativeRound(int RoundNumber, RollRange RollRange, IReadOnlyList<UnitRoll> Rolls);