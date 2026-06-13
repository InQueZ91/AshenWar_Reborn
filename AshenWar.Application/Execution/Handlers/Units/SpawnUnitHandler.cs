using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Application.Services;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Unit;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using Microsoft.Extensions.Logging;

namespace AshenWar.Application.Execution.Handlers.Units;

public sealed class SpawnUnitHandler(ILogger<SpawnUnitHandler> logger, UnitSpawnService unitSpawnService ) 
    : IActionHandler<SpawnUnit>
{
    public async Task Execute(SpawnUnit definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var source = context.Source as IUnit;
        var owner = ResolveOwner(definition, source);
        var spawnPosition = ResolvePosition(definition, source, context);

        if (owner is null || spawnPosition is null)
        {
            logger.LogWarning("Could not resolve owner or spawn position for spawn unit action");
            return;
        }
        
        await unitSpawnService.SpawnAsync(
            context.MatchContext.Match,
            context.MatchContext.TriggerRegistry,
            owner,
            definition.UnitDefinitionId,
            spawnPosition,
            cancellationToken
        );
    }
    
    private UserId? ResolveOwner(SpawnUnit definition, IUnit? source)
    {
        return definition.OwnerSource switch
        {
            OwnerSource.Caster => source?.Owner,
            OwnerSource.Neutral => null,
            OwnerSource.Explicit => definition.ExplicitOwner,
            _ => null
        };
    }
    private HexCoord? ResolvePosition(SpawnUnit definition, IUnit? source, ActionContext context)
    {
        return definition.PositionSource switch
        {
            PositionSource.Caster => source?.Position,
            PositionSource.Target => context.Targets.FirstOrDefault()?.Position,
            PositionSource.Explicit => definition.ExplicitPosition,
            _ => null
        };
    }
}