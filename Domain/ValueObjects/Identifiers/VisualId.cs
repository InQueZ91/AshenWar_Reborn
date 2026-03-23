namespace Domain.ValueObjects.Identifiers;

public sealed class VisualId
{
    public string Value { get; }

    public VisualId(string value)
    {
        Value = value;
    }
    
    public override string ToString() => Value;
    public static implicit operator string(VisualId visualId) => visualId.Value;
}