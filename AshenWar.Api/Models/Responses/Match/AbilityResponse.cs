namespace AshenWar.Api.Models.Responses.Match;

public sealed record AbilityResponse
{
    public Guid AbilityId { get; init; }
    public Guid DefinitionId { get; init; }
    public string Name { get; init; } = "";
    public bool IsReady { get; init; }
    public int RemainingCooldown { get; init; }
    public IReadOnlyList<AbilityStepResponse> Steps { get; init; } = [];
}