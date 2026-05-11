namespace Domain.Entities.Actions.ValueSources;

public sealed class FixedValueSource(float value) : ValueSource
{
    public override float Resolve(ActionContext context) => value;
}