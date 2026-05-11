using System.Collections.Generic;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Entities.Lobbies;

public sealed class LobbySlot
{
    public UserId UserId { get; }
    public PlayerSide Side { get; }
    public IReadOnlyList<UnitDefinitionId> Roster { get; private set; } = [];
    public bool IsReady { get; private set; }
    public bool HasRoster => Roster.Count > 0;

    private LobbySlot(UserId userId, PlayerSide side)
    {
        UserId = userId;
        Side = side;
    }
    
    public static LobbySlot Create(UserId userId, PlayerSide side) => new (userId, side);
    
    public void SelectRoster(IReadOnlyList<UnitDefinitionId> roster)
    {
        if (IsReady)
            throw new DomainException("Cannot change roster after ready.");
        if (roster.Count == 0)
            throw new DomainException("No roster selected.");
        Roster = roster;
    }
    
    public void SetReady()
    {
        if (!HasRoster)
            throw new DomainException("Cannot ready without a roster.");
        IsReady = true;
    }
    
    public void UnReady() => IsReady = false;
}