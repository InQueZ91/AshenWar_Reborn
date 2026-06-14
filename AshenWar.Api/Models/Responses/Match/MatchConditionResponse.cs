namespace AshenWar.Api.Models.Responses.Match;

public sealed record MatchConditionResponse
{
    public Guid DefinitionId { get; init; }
    public int RemainingDuration { get; init; }
    public int Stacks { get; init; }
}