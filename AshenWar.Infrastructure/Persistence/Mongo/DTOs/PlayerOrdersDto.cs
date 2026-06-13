namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record PlayerOrdersDto
{
    public Guid OwnerId { get; init; }
    public required List<UnitOrderDto> Orders { get; init; }
}