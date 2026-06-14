namespace AshenWar.Api.Models.Responses;

public sealed record UnitStatsResponse
{
    public int Power { get; init; }
    public int Health { get; init; }
    public int Stamina { get; init; }
    public int Steps { get; init; }
    public int Speed { get; init; }
    public int Vision { get; init; }
};