using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Infrastructure.Definitions.Dtos.Abilities;

public sealed record AbilityStepDto
{
    public IValidator? Validator { get; init; }
    public required ITargetShape Shape { get; init; }
    public ITargetFilter? Filter { get; init; }
    public IEnumerable<IActionDefinition> Actions { get; init; } = [];
    public IEnumerable<Guid> Effects { get; init; } = [];
}