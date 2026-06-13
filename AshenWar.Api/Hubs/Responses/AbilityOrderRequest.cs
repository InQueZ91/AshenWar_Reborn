using AshenWar.Infrastructure.Persistence.Mongo.Dtos;

namespace AshenWar.Api.Hubs.Responses;

public sealed record AbilityOrderRequest
{
    public Guid AbilityId { get; init; }
    public IReadOnlyList<StepSelectionDto> Selections { get; init; } = [];
}