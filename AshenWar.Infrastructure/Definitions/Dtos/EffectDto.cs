using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Infrastructure.Definitions.Dtos;

public sealed record EffectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public required IEnumerable<IActionDefinition> Actions { get; init; }
}