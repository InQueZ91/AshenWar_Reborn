using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Infrastructure.Definitions.Repositories;

public sealed class InMemoryConditionDefinitionRepository(DefinitionStore store) : IConditionDefinitionRepository
{
    public Task<UnitConditionDefinition> GetUnitConditionAsync(UnitConditionDefinitionId id, CancellationToken cancellationToken = default)
    {
        if (!store.UnitConditions.TryGetValue(id, out var definition))
            throw new DomainException($"UnitConditionDefinition '{id}' not found.'");
        
        return Task.FromResult(definition); 
    }

    public Task<TileConditionDefinition> GetTileConditionAsync(TileConditionDefinitionId id, CancellationToken cancellationToken = default)
    {
        if (!store.TileConditions.TryGetValue(id, out var definition))
            throw new DomainException($"TileConditionDefinition '{id}' not found.'");
        
        return Task.FromResult(definition);
    }

    public Task<GlobalConditionDefinition> GetGlobalConditionAsync(GlobalConditionDefinitionId id, CancellationToken cancellationToken = default)
    {
        if (!store.GlobalConditions.TryGetValue(id, out var definition))
            throw new DomainException($"GlobalConditionDefinition '{id}' not found.'");
        
        return Task.FromResult(definition);
    }
}