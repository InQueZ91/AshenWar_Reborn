using System;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Application.Planning;

public abstract record TimerMessage(MatchId MatchId);

public sealed record StartTimer(MatchId MatchId, DateTimeOffset StartedAt, TimeSpan Duration) : TimerMessage(MatchId);

public sealed record CancelTimer(MatchId MatchId) : TimerMessage(MatchId);