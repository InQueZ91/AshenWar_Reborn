using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Unit;
using Domain.Enums;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Players;

namespace Application.Execution.Handlers.Units;

public sealed class SpawnUnitHandler(UnitSpawnService unitSpawnService) : IActionHandler<SpawnUnit>
{
    public async Task Execute(SpawnUnit definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var source = context.Source as IUnitCommand;
        var owner = ResolveOwner(definition, source);
        var spawnPosition = ResolvePosition(definition, source, context);
        
        await unitSpawnService.SpawnAsync(
            context.MatchContext.Match,
            context.MatchContext.TriggerRegistry,
            owner,
            definition.UnitDefinitionId,
            spawnPosition,
            cancellationToken
        );
    }
    
    private UserId? ResolveOwner(SpawnUnit definition, IUnitCommand? source)
    {
        return definition.OwnerSource switch
        {
            OwnerSource.Caster => source?.Owner,
            OwnerSource.Neutral => null,
            OwnerSource.Explicit => definition.ExplicitOwner,
            _ => null
        };
    }
    private HexCoord? ResolvePosition(SpawnUnit definition, IUnitCommand? source, ActionContext context)
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