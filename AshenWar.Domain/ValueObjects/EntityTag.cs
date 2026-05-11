namespace Domain.ValueObjects;

public readonly record struct EntityTag(string Value)
{
    public static readonly EntityTag Dragon = new("Dragon");
    public static readonly EntityTag Human = new("Human");
    public static readonly EntityTag Fire = new("Fire");
    public static readonly EntityTag Ice = new("Ice");
    public static readonly EntityTag Physical = new("Physical");
    public static readonly EntityTag Movement = new("Movement");
    public static readonly EntityTag Melee = new("Melee");
    public static readonly EntityTag Ranged = new("Ranged");
    public static readonly EntityTag Spell = new("Spell");
    public static readonly EntityTag Aoe = new("Aoe");
    public static readonly EntityTag SingleTarget = new("SingleTarget");
    public static readonly EntityTag Overload = new("Overload");
    public static readonly EntityTag Unite = new("Unite");
}