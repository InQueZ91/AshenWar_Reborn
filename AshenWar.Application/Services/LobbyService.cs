using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Commands.Matches.StartMatch;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Notifications;
using AshenWar.Application.Validators;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Entities.Lobbies;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AshenWar.Application.Services;

public sealed class LobbyService(IServiceScopeFactory scopeFactory, IGameNotifier gameNotifier)
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
        var alreadyInLobby = false;

        lock (_queueLock)
        {
            if (_userLobbyIndex.ContainsKey(userId))
            {
                alreadyInLobby = true;
            }
            else if (_queue.Contains(userId))
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

        if (alreadyQueued || playerOne is null || playerTwo is null)
            return LobbyJoinResult.Waiting();
        
        if (alreadyInLobby)
            return LobbyJoinResult.AlreadyInLobby();

        // ——————— Join Lobby ————————
        // get repositories
        using var scope = scopeFactory.CreateScope();
        var matchDefinitionRepository = scope.ServiceProvider.GetRequiredService<IMatchDefinitionRepository>();
        var mapRepository = scope.ServiceProvider.GetRequiredService<IMapDefinitionRepository>();

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
        await gameNotifier.LobbyFounded(lobby, ct);

        return LobbyJoinResult.Found(lobby.Id);
    }

    public async Task SubmitDeploymentAsync(UserId userId, LobbyId lobbyId, List<DeploymentPlan> deploymentPlans,
        CancellationToken ct)
    {
        var lobby = GetLobby(lobbyId);
        using var scope = scopeFactory.CreateScope();

        // Validate deployment plans
        var deploymentValidator = scope.ServiceProvider.GetRequiredService<DeploymentValidator>();
        await deploymentValidator.Validate(
            deploymentPlans,
            lobby.MatchDefinitionId,
            lobby.MapDefinitionId,
            lobby.GetPlayerSlot(userId).Side,
            ct);

        lobby.SubmitDeployment(userId, deploymentPlans);

        // broadcast ready state to both - but not roster contents
        await gameNotifier.LobbyUpdated(lobby, ct);

        if (!lobby.BothSubmitted) return;

        // Both players are ready - send create match request
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new StartMatchRequest(
            lobby.MatchDefinitionId,
            lobby.MapDefinitionId,
            lobby.Blue.UserId,
            lobby.Blue.DeploymentPlans,
            lobby.Red.UserId,
            lobby.Red.DeploymentPlans
        );

        await mediator.Send(command, ct);

        // cleanup lobby after successful broadcast
        CleanupLobby(lobby);
    }

    public async Task CancelDeploymentAsync(UserId userId, LobbyId lobbyId, CancellationToken ct)
    {
        var lobby = GetLobby(lobbyId);

        if (lobby.BothSubmitted)
            throw new DomainException("Cannot cancel deployment after both players have submitted.");

        lobby.CancelDeployment(userId);

        await gameNotifier.LobbyUpdated(lobby, ct);
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

        CleanupLobby(lobby);
        
        await gameNotifier.LobbyDeleted(lobby, ct);
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
}