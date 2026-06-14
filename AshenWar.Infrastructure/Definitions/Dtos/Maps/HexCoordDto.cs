namespace AshenWar.Infrastructure.Definitions.Dtos.Maps;

public sealed record HexCoordDto
{
    public required int Q { get; init; }
    public required int R { get; init; }
}