using AshenWar.Domain.Enums;

namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record StepSelectionDto
{
    public Guid AbilityStepId { get; init; }
    public Guid? TargetId { get; init; }
    public TargetType TargetType { get; init; }
}