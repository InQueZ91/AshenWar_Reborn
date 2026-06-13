namespace AshenWar.Api.Models.Responses.Definitions.Maps;

public sealed record DeploymentPointsResponse
{
    public required IReadOnlyList<HexCoordResponse> Blue { get; init; }
    public required IReadOnlyList<HexCoordResponse> Red { get; init; }
}