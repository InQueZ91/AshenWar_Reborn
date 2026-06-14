using AshenWar.Domain.Enums;

namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record PlayerDto
{
    public Guid UserId { get; init; }
    public PlayerSide Side { get; init; }
    public required List<Guid> Rosters { get; init; }
}