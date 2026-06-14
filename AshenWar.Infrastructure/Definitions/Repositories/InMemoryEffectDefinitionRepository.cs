using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Effects;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public sealed class InMemoryEffectDefinitionRepository(DefinitionStore store) : IEffectDefinitionRepository
{
    public Task<EffectDefinition> GetEffectAsync(EffectDefinitionId id, CancellationToken cancellationToken = default)
    {
        if (!store.Effects.TryGetValue(id, out var definition))
            throw new DomainException($"EffectDefinition '{id}' not found.'");
        
        return Task.FromResult(definition);  
    }
}