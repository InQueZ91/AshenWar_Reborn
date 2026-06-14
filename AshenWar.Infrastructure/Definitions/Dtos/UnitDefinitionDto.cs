namespace AshenWar.Infrastructure.Definitions.Dtos;

public sealed record UnitDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public Guid VisualId { get; init; }
    public required UnitStatsDto Stats { get; init; }
    public IEnumerable<Guid> Abilities { get; init; } = [];
    public IEnumerable<Guid> Passives { get; init; } = [];
    public IEnumerable<string> Tags { get; init; } = [];
}