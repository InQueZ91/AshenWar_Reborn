namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record TurnDto
{
    public Guid TurnId { get; init; }
    public int TurnNumber { get; init; }
    public DateTimeOffset PlanningStartedAt { get; init; }
    public int PlanningDurationSeconds { get; init; }
    public bool BlueSubmitted { get; init; }
    public bool RedSubmitted { get; init; }
    public bool IsResolved { get; init; }
    public required PlayerOrdersDto BlueOrders { get; init; }
    public required PlayerOrdersDto RedOrders { get; init; }
}