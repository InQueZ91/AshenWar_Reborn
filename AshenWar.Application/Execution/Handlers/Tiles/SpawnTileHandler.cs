using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Tile;
using AshenWar.Domain.Entities.Tiles;

namespace AshenWar.Application.Execution.Handlers.Tiles;

public sealed class SpawnTileHandler(
    ITileDefinitionRepository tileDefinitionRepository,
    IPassiveDefinitionRepository passiveDefinitionRepository) : IActionHandler<SpawnTile>
{
    public async Task Execute(SpawnTile definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var matchContext = context.MatchContext;
        
        // Fetch tile definition
        var tileDefinition = await tileDefinitionRepository.FindAsync(definition.TileDefinitionId, cancellationToken);
        if (tileDefinition is null)
        {
            return;
        }
        
        // Instantiate tile
        var tile = Tile.Instantiate(tileDefinition);
        
        // Instantiate Passive Abilities from repository
        var triggerRegistry = matchContext.TriggerRegistry;
        foreach (var passiveAbilityDefinitionId in tileDefinition.Passives)
        {
            var passiveAbilityDefinition = await passiveDefinitionRepository.FindPassiveAsync(passiveAbilityDefinitionId, cancellationToken);
            if (passiveAbilityDefinition is null)
                continue;
            var passiveAbility = Passive.Instantiate(passiveAbilityDefinition);
            tile.AddPassive(passiveAbility);
            triggerRegistry.Register(tile, passiveAbility);
        }
        
        matchContext.Match.SpawnTile(tile, definition.Position);
    }
}