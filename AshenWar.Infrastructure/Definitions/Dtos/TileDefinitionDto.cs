namespace AshenWar.Infrastructure.Definitions.Dtos;

public sealed record TileDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public Guid VisualId { get; init; }
    public required TileStatsDto Stats { get; init; }
    public IEnumerable<Guid> Passives { get; init; } = [];
    public IEnumerable<string> Tags { get; init; } = [];
}