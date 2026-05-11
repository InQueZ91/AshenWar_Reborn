namespace Domain.Entities.Actions.ValueSources;

public sealed class ScaledValueSource(ValueSource sourceValue, float multiplier) : ValueSource
{
    public override float Resolve(ActionContext context)
        => sourceValue.Resolve(context) * multiplier;
}