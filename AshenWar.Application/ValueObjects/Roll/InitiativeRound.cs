
using System.Collections.Generic;

namespace AshenWar.Application.ValueObjects.Roll;

public sealed record InitiativeRound(int RoundNumber, RollRange RollRange, IReadOnlyList<UnitRoll> Rolls);