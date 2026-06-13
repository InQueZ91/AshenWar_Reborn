namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos.Abilities;

public sealed record AbilityDto
{
    public Guid Id { get; init; }
    public Guid DefinitionId { get; init; }
    public int RemainingCooldown { get; init; }
}