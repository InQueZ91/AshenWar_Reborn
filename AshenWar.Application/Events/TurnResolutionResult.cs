using System.Collections.Generic;
using AshenWar.Application.ValueObjects;
using AshenWar.Application.ValueObjects.Roll;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Application.Events;

public sealed record TurnResolutionResult(
    TurnId TurnId,
    int TurnNumber,
    TurnInitiative Initiative,
    IReadOnlyList<ResolutionBatch> Batches);