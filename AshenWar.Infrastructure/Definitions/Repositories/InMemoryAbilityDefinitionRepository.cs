using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public class InMemoryAbilityDefinitionRepository(DefinitionStore store) : IAbilityDefinitionRepository
{
    public Task<IReadOnlyList<AbilityDefinition>> GetAllAbilityAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<AbilityDefinition>>(store.Abilities.Values.ToList());
    }

    public Task<AbilityDefinition?> FindAbilityAsync(AbilityDefinitionId id, CancellationToken cancellationToken = default)
    {
        store.Abilities.TryGetValue(id, out var definition);
        return Task.FromResult(definition);
    }
}