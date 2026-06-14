namespace AshenWar.Api.Models.Responses.MatchStarted;

public sealed record MatchStartedTileResponse
{
    public Guid TileId { get; init; }
    public Guid DefinitionId { get; init; }
    public HexCoordResponse? Position { get; init; }
    public required TileStatsResponse FinalStats { get; init; }
    public IReadOnlyList<PassiveResponse> Passives { get; init; } = [];
    public IReadOnlyList<ConditionResponse> Conditions { get; init; } = [];
    public bool HasFog { get; init; }
    public bool IsClaimed { get; init; }
}