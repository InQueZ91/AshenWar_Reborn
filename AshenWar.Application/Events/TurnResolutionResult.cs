using System.Collections.Generic;
using Application.ValueObjects;
using Application.ValueObjects.Roll;
using Domain.ValueObjects.Identifiers.Match;

namespace Application.Events;

public sealed record TurnResolutionResult(
    TurnId TurnId,
    int TurnNumber,
    TurnInitiative Initiative,
    IReadOnlyList<ResolutionBatch> Batches);