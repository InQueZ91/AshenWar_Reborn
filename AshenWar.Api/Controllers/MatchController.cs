using System.Security.Claims;
using AshenWar.Api.Models.Responses;
using AshenWar.Api.Models.Responses.Match;
using AshenWar.Application.Contracts;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AshenWar.Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/matches")]
public sealed class MatchController(IMatchRepository matchRepository) : ControllerBase
{
    // --- Helper ---
    private Guid CurrentUserId => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private bool IsParticipant(Match match, Guid userId) =>
        match.BlueSide.UserId.Value == userId || 
        match.RedSide.UserId.Value == userId;
    
    // Called after MatchStarted and after every ResolutionCompleted.
    // Returns full board state = unit positions, current stats, conditions.
    // No definition data - client fetches definitions separately and caches them.
    [HttpGet("{matchId:guid}/state")]
    public async Task<IActionResult> GetMatchStateAsync(Guid matchId, CancellationToken ct)
    {
        var match = await matchRepository.FindAsync(new MatchId(matchId), ct);
        if (match is null) return NotFound();
        if (!IsParticipant(match, CurrentUserId)) return Forbid();

        var units = match.Board.GetAllUnits()
            .Select(u => new MatchUnitResponse
            {
                UnitId = u.Id.Value,
                DefinitionId = u.DefinitionId.Value,
                OwnerId = u.Owner.Value,
                Name = u.Name,
                IsAlive = u.IsAlive,
                CurrentHealth = u.CurrentHealth,
                CurrentStamina = u.CurrentStamina,
                CurrentSteps = u.CurrentSteps,
                Position = u.Position is null ? null : new HexCoordResponse(u.Position.Q, u.Position.R),
                Conditions = u.Conditions
                    .Select(c => new MatchConditionResponse
                    {
                        DefinitionId = c.Definition.Id.Value,
                        RemainingDuration = c.RemainingDuration,
                        Stacks = c.CurrentStacks
                    }).ToList()
            }).ToList();
        
        return Ok(new MatchStateResponse(units));
    }
    
    // Called during planning when a player clicks a unit to issue orders.
    // Returns runtime ability state only — IsReady, RemainingCooldown, step IDs.
    // Client fetches ability definitions separately via /api/definitions/abilities/{id}.
    [HttpGet("{matchId:guid}/units/{unitId:guid}/abilities")]
    public async Task<IActionResult> GetUnitAbilitiesAsync(Guid matchId, Guid unitId, CancellationToken ct)
    {
        var match = await matchRepository.FindAsync(new MatchId(matchId), ct);
        if (match is null) return NotFound();
        if (!IsParticipant(match, CurrentUserId)) return Forbid();

        var unit = match.Board.FindUnitById(new UnitId(unitId));
        if (unit is null) return NotFound();

        var abilities = unit.Abilities
            .Select(a => new AbilityResponse
            {
                AbilityId = a.Id.Value,
                DefinitionId = a.Definition.Id.Value,
                Name = a.Definition.Name,
                IsReady = a.IsReady,
                RemainingCooldown = a.RemainingCooldown,
                Steps = a.Definition.Steps
                    .Select(s => new AbilityStepResponse(s.Id.Value, s.Shape.RequiresInput))
                    .ToList()
            })
            .ToList();

        return Ok(new UnitAbilitiesResponse(abilities));
    }
}