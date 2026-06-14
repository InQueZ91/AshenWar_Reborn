using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Match.Definitions;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public sealed class InMemoryMapDefinitionRepository(DefinitionStore store) : IMapDefinitionRepository
{
    public Task<MapDefinition?> FindAsync(MapDefinitionId id, CancellationToken cancellationToken = default)
    {
        store.Maps.TryGetValue(id, out var definition);
        return Task.FromResult(definition);
    }

    public Task<MapDefinition> GetRandomAsync(CancellationToken cancellationToken = default)
    {
        if (store.Maps.Count == 0)
            throw new DomainException("No maps loaded. Check your /definitions/maps folder");

        var index = Random.Shared.Next(store.Maps.Count);
        var definition = store.Maps.Values.ElementAt(index);
        return Task.FromResult(definition);
    }
}