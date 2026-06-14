namespace AshenWar.Infrastructure.Definitions.Dtos.Abilities;

public sealed record AbilityStatsDto
{
    public int Damage { get; init; }
    public int Cooldown { get; init; }
    public int Range { get; init; }
}