using System;
using System.Collections.Generic;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Entities.Lobbies;

public sealed class Lobby : DomainEntity
{
    public LobbyId Id { get; } = LobbyId.New();
    public MatchDefinitionId MatchDefinitionId { get; }
    public MapDefinitionId MapDefinitionId { get; }
    public PlayerSlot Blue { get; }
    public PlayerSlot Red { get; }
    public bool BothSubmitted => Blue.HasSubmitted && Red.HasSubmitted;

    private Lobby(
        UserId blueUserId,
        UserId redUserId,
        MatchDefinitionId matchDefinitionId,
        MapDefinitionId mapDefinitionId)
    {
        MatchDefinitionId = matchDefinitionId;
        MapDefinitionId = mapDefinitionId;
        Blue = PlayerSlot.Create(blueUserId, PlayerSide.Blue);
        Red = PlayerSlot.Create(redUserId, PlayerSide.Red);
    }

    public static Lobby Create(
        UserId blueUserId,
        UserId redUserId,
        MatchDefinitionId matchDefinitionId,
        MapDefinitionId mapDefinitionId)
    {
        ArgumentNullException.ThrowIfNull(blueUserId);
        ArgumentNullException.ThrowIfNull(redUserId);
        ArgumentNullException.ThrowIfNull(matchDefinitionId);
        ArgumentNullException.ThrowIfNull(mapDefinitionId);
        if (blueUserId == redUserId)
            throw new DomainException("Players must be different users.");
        
        return new Lobby(blueUserId, redUserId, matchDefinitionId, mapDefinitionId);
    }

    public PlayerSlot GetPlayerSlot(UserId userId)
    {
        if (Blue.UserId == userId) return Blue;
        if (Red.UserId == userId) return Red;
        
        throw new DomainException($"User {userId} is not in this lobby.");
    }

    public void SubmitDeployment(UserId userId, List<DeploymentPlan> deploymentPlans) 
        => GetPlayerSlot(userId).SubmitDeployment(deploymentPlans);
    
    public void CancelDeployment(UserId userId)
        => GetPlayerSlot(userId).CancelDeployment();
}