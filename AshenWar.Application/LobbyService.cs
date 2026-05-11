using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Commands.Matches.CreateMatch;
using Application.Contracts;
using Application.Hubs;
using Application.Validators;
using Application.ValueObjects;
using Domain.Entities.Lobbies;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Application;

public sealed class LobbyService(
    IMediator mediator,
    IMatchDefinitionRepository matchDefinitionRepository,
    IMapRepository mapRepository,
    IUnitRepository unitRepository,
    IHubContext<GameHub> hubContext)
{
    private readonly ConcurrentDictionary<UserId, LobbyId> _userLobbyIndex = new();
    private readonly ConcurrentDictionary<LobbyId, Lobby> _lobbies = new();
    
    private readonly List<UserId> _queue = [];
    private readonly Lock _queueLock = new();

    public async Task<LobbyJoinResult> JoinQueueAsync(UserId userId, CancellationToken ct)
    {
        UserId? playerOne = null;
        UserId? playerTwo = null;

        var alreadyQueued = false;
        
        lock (_queueLock)
        {
            if (_queue.Contains(userId) || _userLobbyIndex.ContainsKey(userId))
            {
                alreadyQueued = true;
            }
            else
            {
                _queue.Add(userId);
                
                if (_queue.Count >= 2)
                {
                    playerOne = _queue[0];
                    playerTwo = _queue[1];
                    _queue.RemoveRange(0, 2);
                }
            }
        }

        if (alreadyQueued)
            return LobbyJoinResult.AlreadyInLobby();
        
        if (playerOne is null || playerTwo is null)
            return LobbyJoinResult.Waiting();
        
        // fetch random definitions
        var matchDefinition = await matchDefinitionRepository.GetRandomAsync(ct);
        var mapDefinition = await mapRepository.GetRandomAsync(ct);
        
        // assign sides randomly
        var (blue, red) = Random.Shared.Next(2) == 0 
            ? (playerOne, playerTwo) 
            : (playerTwo, playerOne);

        var lobby = Lobby.Create(blue, red, matchDefinition.Id, mapDefinition.Id);
        
        _lobbies[lobby.Id] = lobby;
        _userLobbyIndex[blue] = lobby.Id;
        _userLobbyIndex[red] = lobby.Id;
        
        // broadcast lobby found to both players
        await hubContext.Clients
            .User(blue.Value.ToString())
            .SendAsync("LobbyFound", new
            {
                LobbyId = lobby.Id,
                MatchDefinitionId = matchDefinition.Id,
                MapDefinitionId = mapDefinition.Id,
                YourSide = PlayerSide.Blue
            }, ct);

        await hubContext.Clients
            .User(red.Value.ToString())
            .SendAsync("LobbyFound", new
            {
                LobbyId = lobby.Id,
                MatchDefinitionId = matchDefinition.Id,
                MapDefinitionId = mapDefinition.Id,
                YourSide = PlayerSide.Red
            }, ct);
        
        return LobbyJoinResult.Found(lobby.Id);
    }

    public async Task SelectRosterAsync(
        UserId userId,
        LobbyId lobbyId,
        IReadOnlyList<UnitDefinitionId> roster,
        CancellationToken ct)
    {
        var lobby = GetLobby(lobbyId);
        
        var matchDefinition = await matchDefinitionRepository.GetAsync(lobby.MatchDefinitionId, ct);
        var rosterDefinitions = await unitRepository.GetManyAsync(roster, ct);
        RosterValidator.Validate(rosterDefinitions, matchDefinition);

        lobby.SelectRoster(userId, roster);
        
        // broadcast only to the player who selected - roster is hidden
        await hubContext.Clients
            .User(userId.Value.ToString())
            .SendAsync("RosterSelected", new { LobbyId = lobbyId }, ct);
    }

    public async Task<bool> SetReadyAsync(UserId userId, LobbyId lobbyId, CancellationToken ct)
    {
        var lobby = GetLobby(lobbyId);
        lobby.SetReady(userId);
        
        // broadcast ready state to both - but not roster contents
        await BroadcastLobbyState(lobby, ct);

        if (!lobby.BothReady) return false;
        
        // both ready => create a match
        var command = new CreateMatchRequest(
            lobby.MatchDefinitionId,
            lobby.MapDefinitionId,
            lobby.Blue.UserId,
            lobby.Blue.Roster,
            lobby.Red.UserId,
            lobby.Red.Roster
        );
        
        var matchId = await mediator.Send(command, ct);
        
        // broadcast match created to both player
        await hubContext.Clients
            .User(lobby.Blue.UserId.Value.ToString())
            .SendAsync("MatchCreated", new { MatchId = matchId }, ct);

        await hubContext.Clients
            .User(lobby.Red.UserId.Value.ToString())
            .SendAsync("MatchCreated", new { MatchId = matchId }, ct);
        
        // cleanup lobby after successful broadcast
        CleanupLobby(lobby);
        
        return true;
    }

    public async Task LeaveAsync(UserId userId, CancellationToken ct)
    {
        if (!_userLobbyIndex.TryGetValue(userId, out var lobbyId))
        {
            // not in a lobby - mark as ghost if they're in the queue
            lock (_queueLock)
            {
                _queue.Remove(userId); // try direct removal first
            }
            return;
        }
        
        var lobby = GetLobby(lobbyId);
        var opponent = lobby.Blue.UserId == userId ? lobby.Red : lobby.Blue;
        
        CleanupLobby(lobby);
        
        // notify opponent lobby collapsed
        await hubContext.Clients
            .User(opponent.UserId.Value.ToString())
            .SendAsync("LobbyCollapsed", ct);
    }

    private Lobby GetLobby(LobbyId lobbyId)
    {
        return !_lobbies.TryGetValue(lobbyId, out var lobby)
            ? throw new DomainException($"Lobby {lobbyId} not found.")
            : lobby;
    }

    private void CleanupLobby(Lobby lobby)
    {
        _lobbies.TryRemove(lobby.Id, out _);
        _userLobbyIndex.TryRemove(lobby.Blue.UserId, out _);
        _userLobbyIndex.TryRemove(lobby.Red.UserId, out _);
    }

    private async Task BroadcastLobbyState(Lobby lobby, CancellationToken ct)
    {
        await hubContext.Clients
            .User(lobby.Blue.UserId.Value.ToString())
            .SendAsync("LobbyStateUpdated", new
            {
                BlueReady = lobby.Blue.IsReady,
                RedReady = lobby.Red.IsReady
            }, ct);

        await hubContext.Clients
            .User(lobby.Red.UserId.Value.ToString())
            .SendAsync("LobbyStateUpdated", new
            {
                BlueReady = lobby.Blue.IsReady,
                RedReady = lobby.Red.IsReady
            }, ct);
    }
}