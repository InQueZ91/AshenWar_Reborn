using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Entities.Abilities.Active;
using Domain.Entities.Abilities.Passive;
using Domain.Entities.Match;
using Domain.Entities.Units;
using Domain.Interfaces;
using Domain.Interfaces.Match;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;

namespace Application;

public sealed class UnitSpawnService(IUnitRepository unitRepository, IAbilityRepository abilityRepository)
{
    public async Task SpawnAsync(
        IMatchCommand match,
        ITriggerRegistry triggerRegistry,
        UserId owner,
        UnitDefinitionId definitionId,
        HexCoord position,
        CancellationToken cancellationToken)
    {
        var unitDefinition = await unitRepository.GetUnitAsync(definitionId, cancellationToken);
        
        var unit = Unit.Instantiate(owner, unitDefinition);

        foreach (var id in unitDefinition.PassiveAbilities)
        {
            var def = await abilityRepository.GetPassiveAbilityAsync(id, cancellationToken);
            var passive = PassiveAbility.Instantiate(def);
            unit.AddPassive(passive);
            triggerRegistry.Register(unit, passive);
        }

        foreach (var id in unitDefinition.ActiveAbilities)
        {
            var def = await abilityRepository.GetActiveAbilityAsync(id, cancellationToken);
            var active = ActiveAbility.Instantiate(def, unit);
            unit.AddActiveAbility(active);
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