using System.Collections.Generic;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Application.ValueObjects.Roll;

public sealed record TurnInitiative(TurnId TurnId, int Seed, IReadOnlyList<InitiativeRound> Rounds, IReadOnlyList<UnitId> FinalOrder);