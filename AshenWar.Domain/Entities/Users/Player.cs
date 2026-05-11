using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Entities.Users;

public sealed class Player
{
    private readonly List<UnitDefinitionId> _roster = [];
    
    public UserId UserId { get; }
    public PlayerSide Side { get; }
    public IReadOnlyList<UnitDefinitionId> Roster => _roster.AsReadOnly();

    private Player(UserId userId, PlayerSide side, IReadOnlyList<UnitDefinitionId> roster)
    {
        UserId = userId;
        Side = side;
        _roster.AddRange(roster);
    }
    public static Player Instantiate(UserId userId, PlayerSide side, IReadOnlyList<UnitDefinitionId> roster)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roster);
        if (roster.Count == 0)
            throw new ArgumentException("Player must have at least one unit", nameof(roster));
        
        return new Player(userId, side, roster);
    }
}