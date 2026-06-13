using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Application.Contracts;

public interface IMatchRepository
{
    Task<Match?> FindAsync(MatchId id, CancellationToken cancellationToken = default);
    Task<List<Match>> GetActivePlanningMatchesAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(Match match, CancellationToken cancellationToken = default);
}