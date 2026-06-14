namespace AshenWar.Api.Models.Responses.Definitions.Tiles;

public sealed record TileResponse
{
    public Guid Id { get; init; }
    public Guid VisualId { get; init; }
    public string Name { get; init; } = "";
    public required TileStatsResponse BaseStats { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
}