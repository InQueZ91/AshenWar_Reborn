using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Match;
using Domain.ValueObjects.Identifiers.Match;

namespace Application.Contracts;

public interface IMatchRepository
{
    Task<Match?> GetMatchAsync(MatchId id, CancellationToken cancellationToken = default);
    Task<Match?> GetActivePlanningMatchAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(Match match, CancellationToken cancellationToken = default);
}