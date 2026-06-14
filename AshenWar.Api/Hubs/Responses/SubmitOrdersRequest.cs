namespace AshenWar.Api.Hubs.Responses;

public sealed record SubmitOrdersRequest
{
    public Guid UnitId { get; init; }
    public IReadOnlyList<AbilityOrderRequest> AbilityOrders { get; init; } = [];
}