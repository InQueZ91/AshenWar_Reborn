namespace AshenWar.Api.Models.Responses.MatchStarted;

public sealed record AbilityResponse
{
    public Guid AbilityId { get; init; }
    public Guid DefinitionId { get; init; }
    public required string Name { get; init; }
    public bool IsReady { get; init; }
    public int RemainingCooldown { get; init; }
}