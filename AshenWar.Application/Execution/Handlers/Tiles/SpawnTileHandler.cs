using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Interfaces;
using Domain.Entities.Abilities.Passive;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Tile;
using Domain.Entities.Tiles;

namespace Application.Execution.Handlers.Tiles;

public sealed class SpawnTileHandler(
    ITileRepository tileRepository,
    IAbilityRepository abilityRepository) : IActionHandler<SpawnTile>
{
    public async Task Execute(SpawnTile definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var matchContext = context.MatchContext;
        
        // Fetch tile definition
        var tileDefinition = await tileRepository.GetTileAsync(definition.TileDefinitionId, cancellationToken);
        
        // Instantiate tile
        var tile = Tile.Instantiate(tileDefinition);
        
        // Instantiate Passive Abilities from repository
        var triggerRegistry = matchContext.TriggerRegistry;
        foreach (var passiveAbilityDefinitionId in tileDefinition.PassiveAbilities)
        {
            var passiveAbilityDefinition = await abilityRepository.GetPassiveAbilityAsync(passiveAbilityDefinitionId, cancellationToken);
            var passiveAbility = PassiveAbility.Instantiate(passiveAbilityDefinition);
            tile.AddPassive(passiveAbility);
            triggerRegistry.Register(tile, passiveAbility);
        }
        
        matchContext.Match.SpawnTile(tile, definition.Position);
    }
}