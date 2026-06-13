using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Infrastructure.Definitions.Dtos.Conditions;

public sealed record ModifierDto
{
    public required IOperation Operation { get; init; }
    public required IStackScaling StackScaling { get; init; }
    public required string StatName { get; init; }
    public required float BaseValue { get; init; }
    public required float PerStackValue { get; init; }
    public IEnumerable<string> TagFilter { get; init; } = [];
    public IValidator? Guard { get; init; }

    public Modifier ToDomain() => new
    (
        Operation,
        StackScaling,
        StatDefinition.FromName(StatName), 
        BaseValue,
        PerStackValue,
        TagFilter.Select(EntityTag.FromName).ToHashSet(),
        Guard
    );
}