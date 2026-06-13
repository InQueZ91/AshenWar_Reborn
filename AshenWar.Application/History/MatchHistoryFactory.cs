using System.Collections.Generic;
using System.Linq;
using AshenWar.Application.History.Records;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Application.History;

public sealed class MatchHistoryFactory
{
    public MatchHistory Create(Match match)
    {
        var players = new Dictionary<UserId, PlayerSide>
        {
            [match.BlueSide.UserId] = PlayerSide.Blue,
            [match.RedSide.UserId] = PlayerSide.Red
        };
        
        var deployment = match.Board
            .GetAllUnits()
            .Select(u => new DeploymentRecord(
                u.DefinitionId.Value,
                u.Owner.Value,
                u.Position!))
            .ToList();
        
        return MatchHistory.Create(match.Id, players, deployment);       
    }
}