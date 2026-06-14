namespace AshenWar.Infrastructure.Definitions.Dtos.Abilities;

public sealed record PassiveDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public required IEnumerable<PassiveTriggerDto> PassiveTriggers { get; init; }
}