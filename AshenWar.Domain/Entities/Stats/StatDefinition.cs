using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Stats;

public sealed class StatDefinition
{
    public StatDefinitionId Id { get; }
    public string Name { get; }
    public float DefaultValue { get; }

    private StatDefinition(string name, float defaultValue)
    {
        Id = StatDefinitionId.New();
        Name = name;
        DefaultValue = defaultValue;
    }
    
    // All known stats declared here as static fields
    public static readonly StatDefinition Health = new ("Health", 0);
    public static readonly StatDefinition Stamina = new ("Stamina", 0);
    public static readonly StatDefinition Steps = new ("Steps", 0);
    public static readonly StatDefinition Speed = new ("Speed", 1);
    public static readonly StatDefinition Vision = new ("Vision", 2);
    public static readonly StatDefinition Power = new ("Power", 0);
    public static readonly StatDefinition Cooldown = new ("Cooldown", 0);
    public static readonly StatDefinition BaseDamage = new ("BaseDamage", 0);
    public static readonly StatDefinition MovementCost = new ("MovementCost", 1);
    
    public static readonly StatDefinition IncomingDamage = new ("IncomingDamage", 0);
}