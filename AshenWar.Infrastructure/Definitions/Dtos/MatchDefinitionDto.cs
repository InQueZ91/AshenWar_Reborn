namespace AshenWar.Infrastructure.Definitions.Dtos;

public sealed record MatchDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public int PlanningDurationSeconds { get; init; }
    public int PowerLimit { get; init; }
    public IEnumerable<Guid> BlacklistedUnits { get; init; } = [];
}