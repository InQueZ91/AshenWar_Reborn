using System;
using System.Collections.Generic;
using AshenWar.Application.History.Records;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Application.History;

public sealed class MatchHistory
{
    public MatchId MatchId { get; private set; }
    public DateTimeOffset StartedAt { get; private set;}
    public DateTimeOffset? EndedAt { get; private set; }
    public UserId? WinnerId { get; private set; }
    public Dictionary<UserId, PlayerSide> Players { get; private set; }
    public List<DeploymentRecord> Deployments { get; private set; }
    public List<TurnRecord> Turns { get; private set; }

    private MatchHistory(MatchId matchId,
        DateTimeOffset startedAt,
        DateTimeOffset? endedAt,
        UserId? winnerId,
        Dictionary<UserId, PlayerSide> players, 
        List<DeploymentRecord> deployments,
        List<TurnRecord> turns)
    {
        MatchId = matchId;
        StartedAt = startedAt;
        EndedAt = endedAt;
        WinnerId = winnerId;
        Players = players;
        Deployments = deployments;
        Turns = turns;
    }

    public static MatchHistory Create(MatchId matchId, Dictionary<UserId, PlayerSide> players, List<DeploymentRecord> deployments)
    {
        ArgumentNullException.ThrowIfNull(matchId);
        
        if (players.Count < 2)
            throw new Exception("Match must have at least 2 players");
        
        if (deployments.Count <= 0)
            throw new Exception("No deployments recorded");
        
        return new MatchHistory(matchId, DateTimeOffset.UtcNow, null, null, players, deployments, []);
    }

    public static MatchHistory Load(MatchId matchId,
        DateTimeOffset startedAt,
        DateTimeOffset? endedAt,
        UserId? winnerId,
        Dictionary<UserId, PlayerSide> players,
        List<DeploymentRecord> deployments,
        List<TurnRecord> turns)
    {
        ArgumentNullException.ThrowIfNull(matchId);
        
        if (players.Count < 2)
            throw new Exception("Match must have at least 2 players");

        if (deployments.Count <= 0)
            throw new Exception("No deployments recorded");

        if (turns.Count <= 0)
            throw new Exception("No turns recorded");
        
        return new MatchHistory(matchId, startedAt, endedAt, winnerId, players, deployments, turns);
    }
    
    public void RecordTurn(TurnRecord turn)
    {
        ArgumentNullException.ThrowIfNull(turn);
        Turns.Add(turn);
    }
    
    public void Close(UserId? winnerId) // null if draw
    {
        EndedAt = DateTimeOffset.UtcNow;
        WinnerId = winnerId;
    }
}