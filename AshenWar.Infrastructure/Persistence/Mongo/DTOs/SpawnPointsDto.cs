using AshenWar.Domain.ValueObjects;

namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record SpawnPointsDto
{
    public required List<HexCoord> Blue { get; init; }
    public required List<HexCoord> Red { get; init; }
}