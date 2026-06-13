namespace AshenWar.Domain.Enums;

public enum OwnerSource
{
    Caster, // inherit from context.Source.Owner
    Neutral, // no owner - PlayerId null
    Explicit, // specific PlayerId defined
}