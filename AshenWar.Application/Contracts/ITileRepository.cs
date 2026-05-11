using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Tiles;
using Domain.ValueObjects.Identifiers.Tiles;

namespace Application.Contracts;

public interface ITileRepository
{
    Task<TileDefinition> GetTileAsync(TileDefinitionId id, CancellationToken cancellationToken = default);
}