using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public sealed class InMemoryTileDefinitionRepository(DefinitionStore store) : ITileDefinitionRepository
{
    public Task<TileDefinition?> FindAsync(TileDefinitionId id, CancellationToken cancellationToken = default)
    {
        store.Tiles.TryGetValue(id, out var tileDefinition);
        return Task.FromResult(tileDefinition);
    }
}