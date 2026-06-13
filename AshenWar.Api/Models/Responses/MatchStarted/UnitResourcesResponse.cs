namespace AshenWar.Api.Models.Responses.MatchStarted;

public sealed record UnitResourcesResponse 
{
    public int Health { get; init; }
    public int Stamina { get; init; }
    public int Steps { get; init; }
}