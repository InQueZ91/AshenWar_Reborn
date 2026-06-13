using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public sealed class InMemoryUnitDefinitionRepository(DefinitionStore store) : IUnitDefinitionRepository
{
    public Task<IReadOnlyList<UnitDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (store.Units.Count == 0)
            throw new DomainException("No units loaded. Check your /definitions/units folder");
        
        return Task.FromResult<IReadOnlyList<UnitDefinition>>(store.Units.Values.ToList());
    }

    public Task<UnitDefinition?> FindAsync(UnitDefinitionId id, CancellationToken cancellationToken = default)
    {
        store.Units.TryGetValue(id, out var definition);
        return Task.FromResult(definition);
    }

    public Task<IReadOnlyList<UnitDefinition>> GetManyAsync(IReadOnlyList<UnitDefinitionId> ids, CancellationToken cancellationToken = default)
    {
        var result = ids
            .Select(id =>
                store.Units.TryGetValue(id, out var definition)
                    ? definition
                    : throw new DomainException($"UnitDefinition '{id}' not found.'")
            ).ToList();

        return Task.FromResult<IReadOnlyList<UnitDefinition>>(result);
    }
}