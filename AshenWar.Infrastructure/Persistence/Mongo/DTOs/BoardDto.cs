namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record BoardDto
{
    public required List<UnitDto> Units { get; init; }
    public required List<TileDto> Tiles { get; init; }
}