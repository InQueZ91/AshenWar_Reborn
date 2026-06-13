using System;
using System.Collections.Generic;
using AshenWar.Application.ValueObjects;

namespace AshenWar.Application.History.Records;

public sealed record TurnRecord(
    Guid MatchId, 
    Guid TurnId,
    int TurnNumber,
    int Seed,
    IReadOnlyList<Guid> InitiativeOrder,
    IReadOnlyList<PlayerOrderRecord> PlayerOrders,
    IReadOnlyList<ResolutionBatch> ResolutionBatches,
    DateTimeOffset ResolvedAt);