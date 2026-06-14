using AshenWar.Application.Contracts;
using AshenWar.Application.History;
using AshenWar.Application.History.Records;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Infrastructure.Persistence.Mongo;

public sealed class NullMongoMatchHistoryRepository : IMatchHistoryRepository
{
    public Task CreateAsync(MatchHistory history, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task AppendTurnAsync(MatchId matchId, TurnRecord record, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task CloseAsync(MatchId matchId, UserId? winnerId, CancellationToken ct = default)
        => Task.CompletedTask;
    
    public Task<MatchHistory?> GetAsync(MatchId matchId, CancellationToken ct = default)
        => Task.FromResult<MatchHistory?>(null);
}