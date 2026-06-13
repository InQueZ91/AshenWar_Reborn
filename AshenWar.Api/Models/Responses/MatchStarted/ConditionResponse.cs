namespace AshenWar.Api.Models.Responses.MatchStarted;

public sealed record ConditionResponse
{
    public Guid ConditionId { get; init; }
    public required string Name { get; init; }
    public int RemainingDuration { get; init; }
    public int CurrentStacks { get; init; }   
}