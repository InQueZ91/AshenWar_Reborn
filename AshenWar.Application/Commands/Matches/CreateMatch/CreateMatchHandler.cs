using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Enums;
using Application.Exceptions;
using Application.Validators;
using Domain.Entities.Conditions.Global;
using Domain.Entities.Match;
using Domain.Entities.Tiles;
using Domain.Entities.Units;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Tiles;
using MediatR;

namespace Application.Commands.Matches.CreateMatch;

public sealed class CreateMatchHandler(
    IMatchRepository matchRepository,
    IMatchDefinitionRepository matchDefinitionRepository,
    IMapRepository mapRepository,
    IUnitRepository unitRepository,
    ITileRepository tileRepository,
    IConditionRepository conditionRepository) 
    : IRequestHandler<CreateMatchRequest, MatchId>
{
    public async Task<MatchId> Handle(CreateMatchRequest request, CancellationToken cancellationToken)
    {
        // 1 Fetch definitions
        var matchDefinition = await matchDefinitionRepository.GetAsync(request.MatchDefinitionId, cancellationToken);
        var mapDefinition = await mapRepository.GetAsync(request.MapDefinitionId, cancellationToken);
        var blueUnitDefinitions = await unitRepository.GetManyAsync(request.BlueRoster, cancellationToken);
        var redUnitDefinitions = await unitRepository.GetManyAsync(request.RedRoster, cancellationToken);
        
        // 2 Validate roster
        RosterValidator.Validate(blueUnitDefinitions, matchDefinition);
        RosterValidator.Validate(redUnitDefinitions, matchDefinition);
        
        // 3 Construct players
        var blueSide = Player.Instantiate(request.BlueUserId, PlayerSide.Blue, request.BlueRoster);
        var redSide = Player.Instantiate(request.RedUserId, PlayerSide.Red, request.RedRoster);
        
        // 4 Construct match
        var match = Match.Create(
            blueSide,
            redSide,
            mapDefinition.SpawnPoints,
            mapDefinition.GlobalEvents,
            matchDefinition.PlanningDuration
        );
        
        // 5 Spawn tiles
        await SpawnTilesAsync(match, mapDefinition, cancellationToken);
        
        // 6 Apply initial global conditions
        await ApplyInitialGlobalConditionsAsync(match, mapDefinition, cancellationToken);

        // 7 Persist
        await matchRepository.SaveAsync(match, cancellationToken);
        
        // 8 Broadcast MatchCreated
        
        return match.Id;
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
                tileDefinition = await tileRepository.GetTileAsync(definitionId, cancellationToken);
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
            var globalConditionDefinition = await conditionRepository.GetGlobalConditionAsync(definitionId, cancellationToken);
            var globalCondition = GlobalCondition.Instantiate(globalConditionDefinition);
            match.ApplyCondition(globalCondition);
        }
    }
}