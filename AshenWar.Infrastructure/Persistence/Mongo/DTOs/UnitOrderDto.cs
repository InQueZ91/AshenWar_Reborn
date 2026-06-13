namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record UnitOrderDto
{
    public Guid UnitId { get; init; }
    public IReadOnlyList<AbilityOrderDto> AbilityOrders { get; init; } = [];
}