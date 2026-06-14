using AshenWar.Api.Converters;
using AshenWar.Api.Models.Responses;
using AshenWar.Api.Models.Responses.Definitions;
using AshenWar.Api.Models.Responses.Definitions.Abilities;
using AshenWar.Api.Models.Responses.Definitions.Maps;
using AshenWar.Api.Models.Responses.Definitions.Passives;
using AshenWar.Api.Models.Responses.Definitions.Tiles;
using AshenWar.Api.Models.Responses.Definitions.Units;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AshenWar.Api.Controllers;

[ApiController]
[Route("/api/definitions")]
public sealed class DefinitionsController(
    IPassiveDefinitionRepository passiveDefinitionRepository,
    IAbilityDefinitionRepository abilityDefinitionRepository,
    IUnitDefinitionRepository unitDefinitionRepository,
    ITileDefinitionRepository tileDefinitionRepository,
    IMapDefinitionRepository mapDefinitionRepository,
    IMatchDefinitionRepository matchDefinitionRepository) : ControllerBase
{
    [HttpGet("units")]
    public async Task<IActionResult> GetAllUnitDefinitions(CancellationToken ct)
    {
        var unitDefinitions = await unitDefinitionRepository.GetAllAsync(ct);
        var abilityDefinitions = await abilityDefinitionRepository.GetAllAbilityAsync(ct);
        var passiveDefinitions = await passiveDefinitionRepository.GetAllPassiveAsync(ct);
        
        var abilityLookup = abilityDefinitions.ToDictionary(a => a.Id, a => a);
        var passiveLookup = passiveDefinitions.ToDictionary(p => p.Id, p => p);
        
        var response = unitDefinitions.Select(unitDefinition => new UnitSummaryResponse
            {
                Id = unitDefinition.Id.Value,
                Name = unitDefinition.Name,
                VisualId = unitDefinition.VisualId.Value,
                BaseStats = DefinitionConverters.ToResponse(unitDefinition.BaseStats),
                Abilities = unitDefinition.Abilities
                    .Where(abilityLookup.ContainsKey)
                    .Select(id => new AbilitySummaryResponse(id.Value, abilityLookup[id].Name))
                    .ToList(),
                Passives = unitDefinition.Passives
                    .Where(passiveLookup.ContainsKey)
                    .Select(id => new PassiveSummaryResponse(id.Value, passiveLookup[id].Name))
                    .ToList(),
                Tags = unitDefinition.Tags.Select(t => t.Name).ToList()
            })
            .ToList();
        
        return Ok(response);      
    }
    
    [HttpGet("matches/{matchDefinitionId:guid}")]
    public async Task<IActionResult> GetMatchDefinition(Guid matchDefinitionId, CancellationToken ct)
    {
        var matchDefinition = await matchDefinitionRepository.FindAsync(new MatchDefinitionId(matchDefinitionId), ct);
        if (matchDefinition is null) return NotFound();

        var response = new MatchDefinitionResponse
        {
            Name = matchDefinition.Name,
            PlanningDurationSeconds = matchDefinition.PlanningDurationSeconds.Seconds,
            PowerLimit = matchDefinition.PowerLimit,
            BlacklistedUnits = matchDefinition.BlacklistedUnits.Select(u => u.Value).ToList(),
        };
        
        return Ok(response);       
    }
    
    [HttpGet("maps/{mapDefinitionId:guid}")]
    public async Task<IActionResult> GetMapDefinition(Guid mapDefinitionId, CancellationToken ct)
    {
        var mapDefinition = await mapDefinitionRepository.FindAsync(new MapDefinitionId(mapDefinitionId), ct);
        if (mapDefinition is null) return NotFound();

        var response = new MapDefinitionResponse
        {
            Name = mapDefinition.Name,
            Tiles = mapDefinition.Tiles
                .Where(kvp => kvp.Value is not null)
                .Select(kvp => new MapTileResponse(new HexCoordResponse(kvp.Key.Q, kvp.Key.R), kvp.Value!.Value))
                .ToList(),
            DeploymentPoints = new DeploymentPointsResponse
            {
                Blue = mapDefinition.DeploymentPoints[PlayerSide.Blue]
                    .Select(h => new HexCoordResponse(h.Q, h.R))
                    .ToList(),
                Red = mapDefinition.DeploymentPoints[PlayerSide.Red]
                    .Select(h => new HexCoordResponse(h.Q, h.R))
                    .ToList(),
            }
        };
        
        return Ok(response);
    }

    [HttpGet("tiles/{tileDefinitionId:guid}")]
    public async Task<IActionResult> GetTileDefinition(Guid tileDefinitionId, CancellationToken ct)
    {
        var tileDefinition = await tileDefinitionRepository.FindAsync(new TileDefinitionId(tileDefinitionId), ct);
        if (tileDefinition is null) return NotFound();

        var response = new TileResponse
        {
            Id = tileDefinition.Id.Value,
            Name = tileDefinition.Name,
            VisualId = tileDefinition.VisualId.Value,
            BaseStats = DefinitionConverters.ToResponse(tileDefinition.BaseStats),
            Tags = tileDefinition.Tags.Select(t => t.Name).ToList()
        };

        return Ok(response);
    }

    [HttpGet("abilities/{abilityDefinitionId:guid}")]
    public async Task<IActionResult> GetAbilityDefinition(Guid abilityDefinitionId, CancellationToken ct)
    {
        var abilityDefinition =
            await abilityDefinitionRepository.FindAbilityAsync(new AbilityDefinitionId(abilityDefinitionId), ct);
        if (abilityDefinition is null) return NotFound();

        var response = new AbilityDefinitionResponse
        {
            Name = abilityDefinition.Name,
            BaseStats = DefinitionConverters.ToResponse(abilityDefinition.Stats),
            Costs = abilityDefinition.Costs.Select(DefinitionConverters.ToResponse).ToList(),
            Steps = abilityDefinition.Steps.Select(s => new AbilityStepResponse(
                    s.Id.Value,
                    s.Shape.RequiresInput)).ToList(),
            Tags = abilityDefinition.Tags.Select(t => t.Name).ToList()
        };

        return Ok(response);
    }
}