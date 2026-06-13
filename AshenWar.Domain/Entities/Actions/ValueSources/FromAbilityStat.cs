using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Domain.Entities.Actions.ValueSources;

public sealed class FromAbilityStat(StatDefinition stat, AbilityDefinitionId abilityDefinitionId) : ValueSource
{
    public StatDefinition Stat { get; } = stat;
    public AbilityDefinitionId AbilityDefinitionId { get; } = abilityDefinitionId;
    public override float Resolve(ActionContext context)
    {
        if (context.Source is not IUnit unit) return 0f;

        var ability = unit.GetAbilityByDefinitionId(AbilityDefinitionId);
        if (ability is null) return 0f;
        
        var baseValue = ability.Definition.Stats.Get(Stat);
        return ModifierCalculator.Calculate(baseValue, Stat, unit.Conditions, context.AbilityTags);
    }
}