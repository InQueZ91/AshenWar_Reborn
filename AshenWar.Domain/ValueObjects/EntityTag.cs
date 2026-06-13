using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AshenWar.Domain.ValueObjects;

public readonly record struct EntityTag
{
    public string Name { get; }

    private EntityTag(string name) => Name = name;

    public static EntityTag FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));
        
        return ByName.TryGetValue(name, out var tag)
            ? tag 
            : throw new ArgumentException($"Unknown tag: '{name}'. Valid tags: {string.Join(", ", ByName.Keys)}");
    }
    
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
    public static readonly EntityTag Frenzy = new("Frenzy");
    public static readonly EntityTag Unite = new("Unite");
    
    private static readonly Dictionary<string, EntityTag> ByName = typeof(EntityTag)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(EntityTag))
        .Select(f => (EntityTag)f.GetValue(null)!)
        .ToDictionary(t => t.Name);
}