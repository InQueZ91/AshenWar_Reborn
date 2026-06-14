using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Application.Services;

public sealed class UnitSpawnService(
    IUnitDefinitionRepository unitDefinitionRepository,
    IAbilityDefinitionRepository abilityDefinitionRepository,
    IPassiveDefinitionRepository passiveDefinitionRepository)
{
    public async Task SpawnAsync(
        IMatch match,
        ITriggerRegistry triggerRegistry,
        UserId owner,
        UnitDefinitionId definitionId,
        HexCoord position,
        CancellationToken cancellationToken)
    {
        var unitDefinition = await unitDefinitionRepository.FindAsync(definitionId, cancellationToken);
        if (unitDefinition is null)
        {
            return;
        }
        
        var unit = Unit.Instantiate(owner, unitDefinition);

        foreach (var id in unitDefinition.Passives)
        {
            var def = await passiveDefinitionRepository.FindPassiveAsync(id, cancellationToken);
            if (def is null)
            {
                continue;
            }
            
            var passive = Passive.Instantiate(def);
            unit.AddPassive(passive);
            triggerRegistry.Register(unit, passive);
        }

        foreach (var id in unitDefinition.Abilities)
        {
            var def = await abilityDefinitionRepository.FindAbilityAsync(id, cancellationToken);
            if (def is null)
            {
                continue;
            }
            var active = Ability.Instantiate(def);
            unit.AddAbility(active);
        }
        
        match.SpawnUnit(unit, position);
    }
    
    public Task SpawnAsync(
        Match match,
        UserId owner,
        UnitDefinitionId definitionId,
        HexCoord position,
        CancellationToken cancellationToken)
        => SpawnAsync(match, match.TriggerRegistry, owner, definitionId, position, cancellationToken);
}