namespace AshenWar.Api.Models.Responses.MatchStarted;

public sealed record MatchStartedResponse
{
    public Guid MatchId { get; init; }
    public IReadOnlyList<MatchStartedUnitResponse> Units { get; init; } = [];
    public IReadOnlyList<MatchStartedTileResponse> Tiles { get; init; } = [];
}