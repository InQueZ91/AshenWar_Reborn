using AshenWar.Domain.Entities.Actions.ValueSources;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;

namespace AshenWar.Domain.Entities.Actions;

public abstract class ValueSource
{
    public abstract float Resolve(ActionContext context);
    
    public static ValueSource Fixed(float value) => new Fixed(value);
    
    public static ValueSource FromUnitStat(StatDefinition statDefinition) => new FromUnitStat(statDefinition);
    
    public static ValueSource ScaledFromUnitStat(StatDefinition statDefinition, float multiplier) 
        => new ScaledFromUnitStat(statDefinition, multiplier);

    public static ValueSource FromAbilityStat(StatDefinition statDefinition, AbilityDefinitionId abilityDefinitionId) =>
        new FromAbilityStat(statDefinition, abilityDefinitionId);
}