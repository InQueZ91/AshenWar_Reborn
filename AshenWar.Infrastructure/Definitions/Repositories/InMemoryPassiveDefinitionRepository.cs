using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public sealed class InMemoryPassiveDefinitionRepository(DefinitionStore store) : IPassiveDefinitionRepository
{
    
    public Task<IReadOnlyList<PassiveDefinition>> GetAllPassiveAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<PassiveDefinition>>(store.Passives.Values.ToList());
    }

    public Task<PassiveDefinition?> FindPassiveAsync(PassiveDefinitionId id, CancellationToken cancellationToken = default)
    {
        store.Passives.TryGetValue(id, out var definition);
        return Task.FromResult(definition);   
    }
}