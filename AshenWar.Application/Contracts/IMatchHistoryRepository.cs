using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.History;
using AshenWar.Application.History.Records;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Application.Contracts;

public interface IMatchHistoryRepository
{
    Task CreateAsync(MatchHistory history, CancellationToken ct = default);
    Task AppendTurnAsync(MatchId matchId, TurnRecord record, CancellationToken ct = default);
    Task CloseAsync(MatchId matchId, UserId? winnerId, CancellationToken ct = default);
    Task<MatchHistory?> GetAsync(MatchId matchId, CancellationToken ct = default);
}