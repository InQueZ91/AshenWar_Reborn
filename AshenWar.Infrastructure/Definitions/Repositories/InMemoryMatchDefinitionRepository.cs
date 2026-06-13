using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Match.Definitions;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public sealed class InMemoryMatchDefinitionRepository(DefinitionStore store) : IMatchDefinitionRepository
{
    public Task<MatchDefinition?> FindAsync(MatchDefinitionId id, CancellationToken cancellationToken = default)
    {
        store.MatchDefinitions.TryGetValue(id, out var definition);
        return Task.FromResult(definition);
    }

    public Task<MatchDefinition> GetRandomAsync(CancellationToken cancellationToken = default)
    {
        if (store.MatchDefinitions.Count == 0)
            throw new DomainException("No match definitions loaded. Check your /definition/matches folder.");
        
        var index = Random.Shared.Next(store.MatchDefinitions.Count);
        var definition = store.MatchDefinitions.Values.ElementAt(index);
        return Task.FromResult(definition);
    }
}