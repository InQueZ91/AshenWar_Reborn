namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record AbilityOrderDto
{
    public Guid AbilityId { get; init; }
    public required List<StepSelectionDto> Selections { get; init; }
}