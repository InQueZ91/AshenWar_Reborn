using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AshenWar.Domain.Entities.Stats;

public sealed class StatDefinition
{
    public string Name { get; }

    private StatDefinition(string name) => Name = name;

    public static StatDefinition FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Stat name cannot be empty.", nameof(name));
        
        return ByName.TryGetValue(name, out var stat)
            ? stat 
            : throw new ArgumentException($"Unknown stat: '{name}'. Valid stats are: {string.Join(", ", ByName.Keys)}");
    }

    // All known stats declared here as static fields
    
    // Unit stats
    public static readonly StatDefinition Power = new ("Power");
    public static readonly StatDefinition Health = new ("Health");
    public static readonly StatDefinition Stamina = new ("Stamina");
    public static readonly StatDefinition Steps = new ("Steps");
    public static readonly StatDefinition Speed = new ("Speed");
    public static readonly StatDefinition Vision = new ("Vision");
    
    // Ability stats
    public static readonly StatDefinition Damage = new ("Damage");
    public static readonly StatDefinition IncomingDamage = new ("IncomingDamage");
    public static readonly StatDefinition Cooldown = new ("Cooldown");
    public static readonly StatDefinition Range = new ("Range");
    
    // Tile stats
    public static readonly StatDefinition MovementCost = new ("MovementCost");
    
    
    private static readonly Dictionary<string, StatDefinition> ByName = typeof(StatDefinition)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(StatDefinition))
        .Select(f => (StatDefinition)f.GetValue(null)!)
        .ToDictionary(s => s.Name);
}