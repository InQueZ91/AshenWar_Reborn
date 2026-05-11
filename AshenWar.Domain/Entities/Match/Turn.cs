using System;
using Domain.Events.Match;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;

namespace Domain.Entities.Match;

/// <summary>
/// Represents one full planning and resolution cycle.
/// Immutable after resolution - Turn is the unit of replay history.
/// </summary>
public sealed class Turn : MatchEntity
{
    public TurnId Id { get; } = TurnId.New();
    public int TurnNumber { get; }
    public DateTimeOffset PlanningStartedAt { get; private set; }
    public TimeSpan PlanningDuration { get; private set; }
    
    public PlayerOrders BlueOrders { get; private set; }
    public PlayerOrders RedOrders { get; private set; }
    
    public bool BlueSubmitted { get; private set; }
    public bool RedSubmitted { get; private set; }
    public bool IsResolved { get; private set; }
    public bool BothSubmitted => BlueSubmitted && RedSubmitted;

    // Constructor
    private Turn(int turnNumber, UserId blueUserId, UserId redUserId)
    {
        TurnNumber = turnNumber;
        BlueOrders = PlayerOrders.Empty(blueUserId);
        RedOrders = PlayerOrders.Empty(redUserId);
    }
    public static Turn Begin(int number, UserId blueUserId, UserId redUserId)
    {
        if (number <= 0)
            throw new ArgumentException("Turn number must be greater than zero.", nameof(number));

        var turn = new Turn(number, blueUserId, redUserId);

        turn.RaiseDomainEvent(new TurnStarted(turn.Id, number));
        return turn;
    }
    
    public void SubmitBlueOrders(PlayerOrders orders)
    {
        if (IsResolved)
            throw new InvalidOperationException($"Turn {TurnNumber} is already resolved.");
        if (BlueSubmitted)
            throw new InvalidOperationException($"Blue already submitted for turn {TurnNumber}.");
        
        BlueOrders = orders;
        BlueSubmitted = true;
    }
    public void SubmitRedOrders(PlayerOrders orders)
    {
        if (IsResolved)
            throw new InvalidOperationException($"Turn {TurnNumber} is already resolved.");
        if (RedSubmitted)
            throw new InvalidOperationException($"Red already submitted for turn {TurnNumber}.");
        
        RedOrders = orders;
        RedSubmitted = true;
    }
    public void MarkAsResolved()
    {
        if (IsResolved)
            throw new InvalidOperationException($"Turn {TurnNumber} is already resolved.");
        
        IsResolved = true;
        RaiseDomainEvent(new TurnResolved(Id, TurnNumber));
    }
    
    public void BeginPlanning(TimeSpan duration)
    {
        PlanningStartedAt = DateTimeOffset.UtcNow;
        PlanningDuration = duration;
        RaiseDomainEvent(new PlanningStarted(Id, TurnNumber, PlanningStartedAt, duration));
    }
}