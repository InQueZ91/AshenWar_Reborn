namespace AshenWar.Domain.Entities.Actions.ValueSources;

public sealed class Fixed(float value) : ValueSource
{
    public float Value { get; } = value; // for serialization
    public override float Resolve(ActionContext context) => Value;
}