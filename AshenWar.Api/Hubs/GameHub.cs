using System.Security.Claims;
using AshenWar.Api.Hubs.Responses;
using AshenWar.Application.Planning;
using AshenWar.Application.Services;
using AshenWar.Domain.Entities.Lobbies;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;
using AshenWar.Domain.ValueObjects.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AshenWar.Api.Hubs;

[Authorize]
public sealed class GameHub(LobbyService lobbyService, PlanningService planningService, IMediator mediator) : Hub
{
    // --- Connection Lifecycle ---
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up lobby if player disconnects mid-lobby
        // LeaveAsync is safe to call even if user is not in lobby
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null)
            await lobbyService.LeaveAsync(new UserId(Guid.Parse(userId)), CancellationToken.None);
        
        await base.OnDisconnectedAsync(exception);
    }

    // --- Matchmaking ---
    public async Task JoinQueue()
    {
        try
        {
            var userId = GetUserId();
            var result = await lobbyService.JoinQueueAsync(userId, CancellationToken.None);
            await Clients.Caller.SendAsync("QueueResult", result);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("LobbyError", e.Message);
        }
    }

    public async Task LeaveQueue()
    {
        try
        {
            var userId = GetUserId();
            await lobbyService.LeaveAsync(userId, CancellationToken.None);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("LobbyError", e.Message);
        }   
    }
    
    // --- Lobby ---
    public async Task SubmitDeployment(Guid lobbyId, List<DeploymentPlanResponse> deploymentPlanResponses)
    {
        try
        {
            var userId = GetUserId();
            var deploymentPlans = deploymentPlanResponses
                .Select(d => new DeploymentPlan(
                    new UnitDefinitionId(d.UnitDefinitionId), 
                    new HexCoord(d.Q, d.R)))
                .ToList();
        
            await lobbyService.SubmitDeploymentAsync(userId, new LobbyId(lobbyId), deploymentPlans, CancellationToken.None);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("LobbyError", e.Message);
        }
    }

    public async Task CancelDeployment(Guid lobbyId)
    {
        try
        {
            var userId = GetUserId();
            await lobbyService.CancelDeploymentAsync(userId, new LobbyId(lobbyId), CancellationToken.None);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("LobbyError", e.Message);
        }
    }
    
    // --- Match group management ---
    public async Task JoinMatch(Guid matchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, matchId.ToString());
    }

    public async Task LeaveMatch(Guid matchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId.ToString());
    }

    // --- Planning ---
    public async Task SubmitOrders(Guid matchId, List<SubmitOrdersRequest> orders)
    {
        try
        {
            var userId = GetUserId();

            var domainOrders = orders
                .Select(o => new UnitOrder
                {
                    UnitId = new UnitId(o.UnitId),
                    AbilityOrders = o.AbilityOrders.Select(ao => new AbilityOrder
                    {
                        AbilityId = new AbilityId(ao.AbilityId),
                        Selections = ao.Selections.Select(s => new StepSelection(
                            new AbilityStepId(s.AbilityStepId),
                            s.TargetType switch
                            {
                                TargetType.Unit when s.TargetId.HasValue => new Target.Unit(
                                    new UnitId(s.TargetId.Value)),
                                TargetType.Tile when s.TargetId.HasValue => new Target.Tile(
                                    new TileId(s.TargetId.Value)),
                                TargetType.Self => new Target.Self(),
                                _ => new Target.Self() // fallback instead of throw
                            })).ToList().AsReadOnly()
                    }).ToList().AsReadOnly()
                }).ToList();

            var result = await planningService.SubmitOrders(userId, new MatchId(matchId), domainOrders, CancellationToken.None);

            if (!result.Success)
                await Clients.Caller.SendAsync("SubmitOrdersFailed", result.Error);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("SubmitOrdersFailed", ex.Message);
        }
    }
    
    // --- Helpers ---
    private UserId GetUserId()
    {
        var raw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? throw new HubException("Unauthorized");
        return new UserId(Guid.Parse(raw));
    }
}