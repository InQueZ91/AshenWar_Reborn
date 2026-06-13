using AshenWar.Domain.ValueObjects;

namespace AshenWar.Infrastructure.Definitions.Dtos.Maps;

public sealed record TileEntryDto
{
    public required HexCoord Coord { get; init; }
    public required Guid TileDefinitionId { get; init; }
}