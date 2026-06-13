using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Domain.ValueObjects;

public sealed record MatchOutcome
{
    public bool IsOver { get; }
    public UserId? WinnerId { get; }

    private MatchOutcome(bool isOver, UserId? winnerId)
    {
        IsOver = isOver;
        WinnerId = winnerId;
    }
    
    public static MatchOutcome Ongoing() => new (false, null);
    public static MatchOutcome Winner(UserId winnerId) => new(true, winnerId);
    public static MatchOutcome Draw() => new (true, null);
}