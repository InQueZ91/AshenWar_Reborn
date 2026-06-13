using AshenWar.Api.Models.Responses;
using AshenWar.Api.Models.Responses.MatchStarted;
using AshenWar.Application.Contracts.Notifications;
using AshenWar.Application.Events;
using AshenWar.Application.ValueObjects;
using AshenWar.Application.ValueObjects.Roll;
using AshenWar.Domain.Entities.Lobbies;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;
using Microsoft.AspNetCore.SignalR;

namespace AshenWar.Api.Hubs;

public sealed class GameNotifier(IHubContext<GameHub> hubContext) : IGameNotifier
{
    public async Task LobbyFounded(Lobby lobby, CancellationToken ct)
    {
        await hubContext.Clients
            .User(lobby.Blue.UserId.ToString())
            .SendAsync("LobbyFounded", new
            {
                LobbyId = lobby.Id,
                lobby.MatchDefinitionId,
                lobby.MapDefinitionId,
                YourSide = PlayerSide.Blue
            }, ct);

        await hubContext.Clients
            .User(lobby.Red.UserId.ToString())
            .SendAsync("LobbyFounded", new
            {
                LobbyId = lobby.Id,
                lobby.MatchDefinitionId,
                lobby.MapDefinitionId,
                YourSide = PlayerSide.Red
            }, ct);
    }

    public async Task LobbyDeleted(Lobby lobby, CancellationToken ct)
    {
        await hubContext.Clients
            .User(lobby.Blue.UserId.ToString())
            .SendAsync("LobbyDeleted", ct);

        await hubContext.Clients
            .User(lobby.Red.UserId.ToString())
            .SendAsync("LobbyDeleted", ct);
    }

    public async Task LobbyUpdated(Lobby lobby, CancellationToken ct)
    {
        await hubContext.Clients
            .User(lobby.Blue.UserId.ToString())
            .SendAsync("LobbyUpdated", new
            {
                BlueStatus = lobby.Blue.HasSubmitted,
                RedStatus = lobby.Red.HasSubmitted
            }, ct);

        await hubContext.Clients
            .User(lobby.Red.UserId.ToString())
            .SendAsync("LobbyUpdated", new
            {
                BlueStatus = lobby.Blue.HasSubmitted,
                RedStatus = lobby.Red.HasSubmitted
            }, ct);
    }

    public async Task MatchStarted(MatchId matchId, Match match, CancellationToken ct)
    {
        var payload = new MatchStartedResponse
        {
            MatchId = matchId.Value,
            Units = match.Board.GetAllUnits().Select(u => new MatchStartedUnitResponse
            {
                UnitId = u.Id.Value,
                OwnerId = u.Owner.Value,
                DefinitionId = u.DefinitionId.Value,
                Position = u.Position is { } pos
                    ? new HexCoordResponse(pos.Q, pos.R)
                    : throw new InvalidOperationException($"Unit {u.Id} has no position at match start"),
                FinalStats = new UnitStatsResponse
                {
                    Power = u.GetFinalStat(StatDefinition.Power),
                    Health = u.GetFinalStat(StatDefinition.Health),
                    Speed = u.GetFinalStat(StatDefinition.Speed),
                    Stamina = u.GetFinalStat(StatDefinition.Stamina),
                    Steps = u.GetFinalStat(StatDefinition.Steps),
                    Vision = u.GetFinalStat(StatDefinition.Vision)
                },
                Resources = new UnitResourcesResponse
                {
                    Health = u.CurrentHealth,
                    Stamina = u.CurrentStamina,
                    Steps = u.CurrentSteps
                },
                Abilities = u.Abilities.Select(a => new AbilityResponse
                {
                    AbilityId = a.Id.Value,
                    DefinitionId = a.Definition.Id.Value,
                    Name = a.Definition.Name,
                    IsReady = a.IsReady,
                    RemainingCooldown = a.RemainingCooldown
                }).ToList(),
                Passives = u.Passives.Select(p => new PassiveResponse
                {
                    PassiveId = p.Id.Value,
                    DefinitionId = p.Definition.Id.Value,
                    Name = p.Definition.Name
                }).ToList(),
                Conditions = u.Conditions.Select(c => new ConditionResponse
                {
                    ConditionId = c.Id.Value,
                    Name = c.Definition.Name,
                    RemainingDuration = c.RemainingDuration,
                    CurrentStacks = c.CurrentStacks
                }).ToList()
            }).ToList(),
            Tiles = match.Board.GetAllTiles().Select(t => new MatchStartedTileResponse
            {
                TileId = t.Id.Value,
                DefinitionId = t.DefinitionId.Value,
                Position = t.Position is { } pos
                    ? new HexCoordResponse(pos.Q, pos.R)
                    : throw new InvalidOperationException($"Tile {t.Id} has no position at match start"),
                FinalStats = new TileStatsResponse
                {
                    MovementCost = t.GetFinalStat(StatDefinition.MovementCost)
                },
                Passives = t.Passives.Select(p => new PassiveResponse
                {
                    PassiveId = p.Id.Value,
                    DefinitionId = p.Definition.Id.Value,
                    Name = p.Definition.Name
                }).ToList(),
                Conditions = t.Conditions.Select(c => new ConditionResponse
                {
                    ConditionId = c.Id.Value,
                    Name = c.Definition.Name,
                    RemainingDuration = c.RemainingDuration,
                    CurrentStacks = c.CurrentStacks
                }).ToList(),
                HasFog = t.HasFog,
                IsClaimed = t.IsClaimed,
            }).ToList()
        };

        await hubContext.Clients
            .User(match.BlueSide.UserId.ToString())
            .SendAsync("MatchStarted", payload, ct);
        
        await hubContext.Clients
            .User(match.RedSide.UserId.ToString())
            .SendAsync("MatchStarted", payload, ct);
    }

    public async Task PlanningStarted(MatchId matchId, Turn nextTurn, CancellationToken ct)
    {
        await hubContext.Clients
            .Group(matchId.ToString())
            .SendAsync("PlanningStarted", new
            {
                nextTurn.Id,
                nextTurn.TurnNumber,
                nextTurn.PlanningStartedAt,
                nextTurn.PlanningDuration,
                ServerNow = DateTimeOffset.UtcNow
            }, ct);
    }

    public async Task PlanningExpired(MatchId matchId, CancellationToken ct)
    {
        await hubContext.Clients
            .Group(matchId.ToString())
            .SendAsync("PlanningExpired", new PlanningExpired(matchId), ct);
    }

    public async Task TurnInitiativeRolled(MatchId matchId, TurnInitiative initiative, CancellationToken ct)
    {
        await hubContext.Clients
            .Group(matchId.ToString())
            .SendAsync("TurnInitiativeRolled", initiative, ct);
    }

    public async Task ResolutionCompleted(MatchId matchId, TurnResolutionResult result, CancellationToken ct)
    {
        await hubContext.Clients
            .Group(matchId.ToString())
            .SendAsync("ResolutionCompleted", result, ct);
    }

    public async Task MatchEnded(MatchId matchId, UserId? winnerId, CancellationToken ct)
    {
        await hubContext.Clients
            .Group(matchId.ToString())
            .SendAsync("MatchEnded", winnerId, ct);
    }

    public async Task AbilityFailed(MatchId matchId,
        UnitId sourceId,
        AbilityId abilityId,
        List<string> failures,
        CancellationToken ct)
    {
        await hubContext.Clients
            .Group(matchId.ToString())
            .SendAsync("AbilityFailed", new
            {
                UnitId = sourceId,
                AbilityId = abilityId,
                Failures = failures
            }, ct);
    }
}