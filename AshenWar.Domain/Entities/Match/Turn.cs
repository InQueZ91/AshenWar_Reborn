using System;
using AshenWar.Domain.Events.Match;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Domain.Entities.Match;

/// <summary>
/// Represents one full planning and resolution cycle.
/// Immutable after resolution - Turn is the unit of replay history.
/// </summary>
public sealed class Turn : DomainEntity
{
    public TurnId Id { get; }
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
    private Turn(
        TurnId id,
        int turnNumber,
        DateTimeOffset planningStartedAt,
        TimeSpan planningDurationSeconds,
        PlayerOrders blueOrders,
        PlayerOrders redOrders,
        bool blueSubmitted,
        bool redSubmitted,
        bool isResolved)
    {
        Id = id;
        TurnNumber = turnNumber;
        PlanningStartedAt = planningStartedAt;
        PlanningDuration = planningDurationSeconds;
        BlueOrders = blueOrders;
        RedOrders = redOrders;
        BlueSubmitted = blueSubmitted;
        RedSubmitted = redSubmitted;
        IsResolved = isResolved;
    }
    
    public static Turn Instantiate(
        int number,
        UserId blueUserId,
        UserId redUserId,
        TimeSpan planningDurationSeconds)
    {
        ArgumentNullException.ThrowIfNull(blueUserId);
        ArgumentNullException.ThrowIfNull(redUserId);
        
        if (number <= 0)
            throw new ArgumentException("Turn number must be greater than zero.", nameof(number));

        var turn = new Turn(
            TurnId.New(),
            number,
            DateTimeOffset.UtcNow,
            planningDurationSeconds,
            PlayerOrders.Empty(blueUserId),
            PlayerOrders.Empty(redUserId),
            false,
            false,
            false);

        turn.RaiseDomainEvent(new TurnStarted(turn.Id, number));

        return turn;
    }

    public static Turn Rehydrate(
        TurnId id,
        int turnNumber,
        DateTimeOffset planningStartedAt,
        TimeSpan planningDurationSeconds,
        PlayerOrders blueOrders,
        PlayerOrders redOrders,
        bool blueSubmitted,
        bool redSubmitted,
        bool isResolved)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(blueOrders);
        ArgumentNullException.ThrowIfNull(redOrders);
        
        if (turnNumber <= 0)
            throw new DomainException("Turn number must be greater than zero.");
        
        if (planningDurationSeconds < TimeSpan.Zero)
            throw new DomainException("Planning duration must be positive.");
        
        if (isResolved && !(blueSubmitted || redSubmitted))
            throw new DomainException("Turn cannot be resolved if either player has not submitted.");

        return new Turn(id,
            turnNumber,
            planningStartedAt,
            planningDurationSeconds,
            blueOrders,
            redOrders,
            blueSubmitted,
            redSubmitted,
            isResolved);
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