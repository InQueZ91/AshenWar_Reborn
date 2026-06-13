namespace AshenWar.Api.Models.Responses.Definitions.Maps;

public sealed record MapDefinitionResponse
{
    public string Name { get; init; } = "";
    public required IReadOnlyList<MapTileResponse> Tiles { get; init; }
    public required DeploymentPointsResponse DeploymentPoints { get; init; }
}