using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Match.Definitions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Application.Contracts.Definitions;

public interface IMapDefinitionRepository
{
    Task<MapDefinition?> FindAsync(MapDefinitionId id, CancellationToken cancellationToken = default);
    Task<MapDefinition> GetRandomAsync(CancellationToken cancellationToken = default);
}