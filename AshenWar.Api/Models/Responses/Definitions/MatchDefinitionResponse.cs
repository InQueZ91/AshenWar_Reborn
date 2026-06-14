namespace AshenWar.Api.Models.Responses.Definitions;

public sealed record MatchDefinitionResponse
{
    public string Name { get; init; } = "";
    public int PlanningDurationSeconds { get; init; }
    public int PowerLimit { get; init; }
    public IReadOnlyList<Guid> BlacklistedUnits { get; init; } = [];
}