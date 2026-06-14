using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Application.Contracts.Definitions;

public interface ITileDefinitionRepository
{
    Task<TileDefinition?> FindAsync(TileDefinitionId id, CancellationToken cancellationToken = default);
}