using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Entities.Lobbies;

public sealed class Lobby : MatchEntity
{
    public LobbyId Id { get; } = LobbyId.New();
    public MatchDefinitionId MatchDefinitionId { get; }
    public MapDefinitionId MapDefinitionId { get; }
    public LobbySlot Blue { get; }
    public LobbySlot Red { get; }
    public bool BothReady => Blue.IsReady && Red.IsReady;

    private Lobby(
        UserId blueUserId,
        UserId redUserId,
        MatchDefinitionId matchDefinitionId,
        MapDefinitionId mapDefinitionId)
    {
        MatchDefinitionId = matchDefinitionId;
        MapDefinitionId = mapDefinitionId;
        Blue = LobbySlot.Create(blueUserId, PlayerSide.Blue);
        Red = LobbySlot.Create(redUserId, PlayerSide.Red);
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

    public LobbySlot GetSlot(UserId userId)
    {
        if (Blue.UserId == userId) return Blue;
        if (Red.UserId == userId) return Red;
        
        throw new DomainException($"User {userId} is not in this lobby.");
    }
    
    public void SelectRoster(UserId userId, IReadOnlyList<UnitDefinitionId> roster)
        => GetSlot(userId).SelectRoster(roster);
    
    public void SetReady(UserId userId) 
        => GetSlot(userId).SetReady();
    
    public void UnReady(UserId userId)
        => GetSlot(userId).UnReady();
}