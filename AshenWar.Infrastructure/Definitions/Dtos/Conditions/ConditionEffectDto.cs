using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Infrastructure.Definitions.Dtos.Conditions;

public sealed record ConditionEffectDto
{
    public ITargetFilter? Filter { get; init; }
    public IValidator? Guard { get; init; }
    public IEnumerable<IActionDefinition> Actions { get; init; } = [];
}