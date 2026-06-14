namespace AshenWar.Api.Models.Responses.Definitions.Abilities;

public sealed record AbilityStatsResponse
{
    public int Damage { get; init; }
    public int Cooldown { get; init; }
    public int Range { get; init; }
}