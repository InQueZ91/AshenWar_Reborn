using AshenWar.Domain.Entities.Abilities.Passives.Triggers;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Infrastructure.Definitions.Dtos.Abilities;

public sealed record PassiveTriggerDto
{
    public ITargetShape? Shape { get; init; }
    public ITargetFilter? Filter { get; init; }
    public IValidator? Guard { get; init; }
    
    public required IEnumerable<string> Tags { get; init; } // EntityTag
    public required IEnumerable<TriggerEntry> Triggers { get; init; }
    public IEnumerable<IActionDefinition> Actions { get; init; } = [];
    public IEnumerable<Guid> Effects { get; init; } = [];
}