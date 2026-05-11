using System.Collections.Generic;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Units;

namespace Application.ValueObjects.Roll;

public sealed record TurnInitiative(TurnId TurnId, int Seed, IReadOnlyList<InitiativeRound> Rounds, IReadOnlyList<UnitId> FinalOrder);