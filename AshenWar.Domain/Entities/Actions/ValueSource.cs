using Domain.Entities.Actions.ValueSources;
using Domain.Entities.Stats;

namespace Domain.Entities.Actions;

public abstract class ValueSource
{
    public abstract float Resolve(ActionContext context);
    
    public static ValueSource Fixed(float value) => new FixedValueSource(value);
    
    public static ValueSource FromStat(StatDefinition statDefinition) => new FromStatValueSource(statDefinition);
    
    public static ValueSource Scaled(ValueSource sourceValue, float multiplier) => new ScaledValueSource(sourceValue, multiplier);
}