using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Match;
using Domain.ValueObjects.Identifiers.Match;

namespace Application.Contracts;

public interface IMatchDefinitionRepository
{
    Task<MatchDefinition> GetAsync(MatchDefinitionId id, CancellationToken cancellationToken = default);
    Task<MatchDefinition> GetRandomAsync(CancellationToken cancellationToken = default);
}