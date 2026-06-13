using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Actions.ValueSources;

public sealed class ScaledFromUnitStat(StatDefinition stat, float multiplier) : ValueSource
{
    public StatDefinition Stat { get; } = stat; // For serialization
    public float Multiplier { get; } = multiplier; // For serialization
    public override float Resolve(ActionContext context)
    {
        if (context.Source is not IUnit unit)
            return 0f;

        var baseValue = unit.Stats.Get(Stat);
        var modified = ModifierCalculator.Calculate(baseValue, Stat, unit.Conditions, context.AbilityTags);
        return modified * Multiplier;
    }
}