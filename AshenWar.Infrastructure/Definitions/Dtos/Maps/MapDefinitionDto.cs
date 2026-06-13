namespace AshenWar.Infrastructure.Definitions.Dtos.Maps;

public sealed record MapDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public int Radius { get; init; }
    public required IEnumerable<TileEntryDto> Tiles { get; init; }
    public Dictionary<string, List<HexCoordDto>>? DeploymentPoints { get; init; }
    public IEnumerable<Guid> InitialGlobalConditions { get; init; } = [];
    public IEnumerable<GlobalEventDto> GlobalEvents { get; init; } = [];
}