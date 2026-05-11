using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Match;
using Domain.ValueObjects.Identifiers.Match;

namespace Application.Contracts;

public interface IMapRepository
{
    Task<MapDefinition> GetAsync(MapDefinitionId id, CancellationToken cancellationToken = default);
    Task<MapDefinition> GetRandomAsync(CancellationToken cancellationToken = default);
}