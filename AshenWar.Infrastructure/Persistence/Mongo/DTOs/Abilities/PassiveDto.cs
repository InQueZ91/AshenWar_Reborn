namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos.Abilities;

public sealed record PassiveDto()
{
    public Guid Id { get; init; }
    public Guid DefinitionId { get; init; }
}