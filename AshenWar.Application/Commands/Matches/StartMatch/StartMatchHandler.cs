using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Notifications;
using AshenWar.Application.History;
using AshenWar.Application.Planning;
using AshenWar.Application.Services;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Lobbies;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Match.Definitions;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AshenWar.Application.Commands.Matches.StartMatch;

public sealed class StartMatchHandler(
    UnitSpawnService unitSpawnService,
    IGameNotifier gameNotifier,
    IMatchRepository matchRepository,
    IMatchDefinitionRepository matchDefinitionRepository,
    IMapDefinitionRepository mapDefinitionRepository,
    ITileDefinitionRepository tileDefinitionRepository,
    IConditionDefinitionRepository conditionDefinitionRepository,
    IMatchHistoryRepository matchHistoryRepository,
    MatchHistoryFactory matchHistoryFactory,
    ILogger<StartMatchHandler> logger,
    ChannelWriter<TimerMessage> writer) 
    : IRequestHandler<StartMatchRequest, MatchId>
{
    public async Task<MatchId> Handle(StartMatchRequest request, CancellationToken ct)
    {
        // 1 Fetch definitions
        var matchDefinition = await matchDefinitionRepository.FindAsync(request.MatchDefinitionId, ct);
        var mapDefinition = await mapDefinitionRepository.FindAsync(request.MapDefinitionId, ct);
        
        // 2 Construct players
        var blueRoster = request.BlueDeploymentPlan.Select(d => d.UnitDefinitionId).ToList();
        var blueSide = Player.Create(request.BlueUserId, PlayerSide.Blue, blueRoster);
        
        var redRoster = request.RedDeploymentPlan.Select(d => d.UnitDefinitionId).ToList();
        var redSide = Player.Create(request.RedUserId, PlayerSide.Red, redRoster);
        
        // 3 Construct match
        var match = Match.Instantiate(
            blueSide,
            redSide,
            mapDefinition.DeploymentPoints,
            mapDefinition.GlobalEvents,
            matchDefinition.PlanningDurationSeconds
        );
        
        // 4 Spawn tiles
        await SpawnTilesAsync(match, mapDefinition, ct);
        
        // 5 Spawn units
        await SpawnUnitsAsync(match, blueSide, request.BlueDeploymentPlan, ct);
        await SpawnUnitsAsync(match, redSide, request.RedDeploymentPlan, ct);
        
        // 6 Apply initial global conditions
        await ApplyInitialGlobalConditionsAsync(match, mapDefinition, ct);

        // 7 Persist
        await matchRepository.SaveAsync(match, ct);
        
        // 8 Start match
        match.StartMatch();
        var firstTurn = match.CurrentTurn;
        
        // 9 Record history
        // Create history record now - board is fully populated, match is started
        var history = matchHistoryFactory.Create(match);
        await matchHistoryRepository.CreateAsync(history, ct);
           
        // 10 Save match and notify
        await matchRepository.SaveAsync(match, ct);
            
        if (!writer.TryWrite(new StartTimer(match.Id, match.CurrentTurn.PlanningStartedAt, match.PlanningDurationSeconds)))
            logger.LogWarning("Failed to write timer message for match {MatchId}", match.Id);
            
        await gameNotifier.MatchStarted(match.Id, match, ct);
        await gameNotifier.PlanningStarted(match.Id, firstTurn, ct);
        
        return match.Id;
    }

    private async Task SpawnUnitsAsync(Match match, Player player, IReadOnlyList<DeploymentPlan> deploymentPlans,
        CancellationToken cancellationToken)
    {
        foreach (var (unitDefinitionId, position) in deploymentPlans)
        {
            await unitSpawnService.SpawnAsync(
                match,
                player.UserId,
                unitDefinitionId,
                position,
                cancellationToken
            );
        }
    }
    
    private async Task SpawnTilesAsync(Match match, MapDefinition mapDefinition, CancellationToken cancellationToken) 
    {
        var tileDefinitionCaches = new Dictionary<TileDefinitionId, TileDefinition>();
        foreach (var (hexCoord, definitionId) in mapDefinition.Tiles)
        {
            // Skip void tiles
            if (definitionId is null) continue;

            // Cache tile definitions
            if (!tileDefinitionCaches.TryGetValue(definitionId, out var tileDefinition))
            {
                tileDefinition = await tileDefinitionRepository.FindAsync(definitionId, cancellationToken);
                tileDefinitionCaches[definitionId] = tileDefinition;
            }
            
            // Spawn tile
            var tile = Tile.Instantiate(tileDefinition);
            match.SpawnTile(tile, hexCoord);
        }
    }

    private async Task ApplyInitialGlobalConditionsAsync(Match match, MapDefinition mapDefinition,
        CancellationToken cancellationToken) 
    {
        foreach (var definitionId in mapDefinition.InitialGlobalConditions)
        {
            var globalConditionDefinition = await conditionDefinitionRepository.GetGlobalConditionAsync(definitionId, cancellationToken);
            var globalCondition = GlobalCondition.Instantiate(globalConditionDefinition);
            match.ApplyCondition(globalCondition);
        }
    }
}