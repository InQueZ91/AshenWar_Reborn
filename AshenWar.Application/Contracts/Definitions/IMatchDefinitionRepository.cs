using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Match.Definitions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Application.Contracts.Definitions;

public interface IMatchDefinitionRepository
{
    Task<MatchDefinition?> FindAsync(MatchDefinitionId id, CancellationToken cancellationToken = default);
    Task<MatchDefinition> GetRandomAsync(CancellationToken cancellationToken = default);
}