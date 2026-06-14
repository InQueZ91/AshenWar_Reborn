namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos.Conditions;

public sealed record ConditionDto
{
    public Guid Id { get; init; }
    public Guid DefinitionId { get; init; }
    public int RemainingDuration { get; init; }
    public int RemainingStacks { get; init; }
}