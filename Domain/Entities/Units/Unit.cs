using System;
using System.Collections.Generic;
using System.Numerics;
using Domain.Entities.Unit.Events;
using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Unit;

public sealed class Unit : Entity
{
    #region References

    public PlayerId Owner { get; private set; }
    public Vector2 Coord { get; private set; }
    public string Name { get; private set; }
    
    // StatusInstances
    // AbilityInstances

    #endregion

    #region Stats

    public UnitStats BaseStats { get; private set; }
    
    // Resources
    public int Health { get; private set; }
    public int Stamina { get; private set; }
    public int Steps { get; private set; }
    
    // Attributes
    public int Power { get; private set; }
    public int Speed { get; private set; }
    public int Vision { get; private set; }
    
    private readonly List<StatModifier> _statModifiers = new List<StatModifier>();
    
    #endregion

    // State
    public UnitState State { get; private set; }

    // Constructor
    private Unit(PlayerId owner, Vector2 coord, string name, UnitStats baseStats)
    {
        // References
        Owner = owner;
        Coord = coord;
        Name = name;

        // Stats
        BaseStats = baseStats;
        Power = baseStats.Power;
        Health = baseStats.Health;
        Stamina = baseStats.Stamina;
        Steps = baseStats.Steps;
        Speed = baseStats.Speed;
        Vision = baseStats.Vision;
    }
    public static Unit Create(PlayerId owner, Vector2 coord, string name, UnitStats baseStats)
    {
        return new Unit(owner, coord, name, baseStats);
    }

    #region Modifier Methods 

    // Modifiers should never be added/removed directly
    // They should only be added/removed, and stats are recalculated dynamically
    public void AddStatModifier(StatModifier modifier)
    {
        _statModifiers.Add(modifier);
    }
    
    // This method designed to use when 
    // An ability instance creates a modifier
    // A status effect expires
    // A temporary buff/debuff ends
    public void RemoveStatModifier(Guid modifierId)
    {
        _statModifiers.RemoveAll(m => m.Id == modifierId);
    }
    
    // This method designed to use with status effects
    // All related modifiers should be removed
    public void RemoveStatModifiersFromSource(IModifierSource source)
    {
        _statModifiers.RemoveAll(m => m.Source == source);   
    }
    
    #endregion

    #region Intent methods

    // Health
    public void TakeDamage(int amount)
    {
        Health = Math.Max(Health - amount, 0);

        RaiseDomainEvent(new UnitDamaged());
    }
    public void RestoreHealth(int amount)
    {
        Health = Math.Min(Health + amount, GetMaxStat(UnitStat.Health));

        RaiseDomainEvent(new UnitHealthRestored());
    }

    // Stamina
    public void ConsumeStamina(int amount)
    {
        Stamina = Math.Max(Stamina - amount, 0);

        RaiseDomainEvent(new UnitStaminaConsumed());
    }
    public void RestoreStamina(int amount)
    {
        Stamina = Math.Min(Stamina + amount, GetMaxStat(UnitStat.Stamina));

        RaiseDomainEvent(new UnitStaminaRestored());
    }

    // Steps
    public void ConsumeSteps(int amount)
    {
        Steps = Math.Max(Steps - amount, 0);

        RaiseDomainEvent(new UnitStepConsumed());
    }
    public void RestoreSteps(int amount)
    {
        Steps = Math.Min(Steps + amount, GetMaxStat(UnitStat.Steps));

        RaiseDomainEvent(new UnitStepRestored());
    }

    #endregion
    
    // Stats are calculated dynamically based on modifiers
    public int GetMaxStat(UnitStat stat)
    {
        var baseValue = GetBaseStat(stat);
        var flat = 0;
        var multiplier = 1f;
        
        foreach (var modifier in _statModifiers)
        {
            if (modifier.Stat != stat)
                continue;
            
            if (modifier.Type == ModifierType.Flat)
                flat += (int) modifier.Value;
            
            if (modifier.Type == ModifierType.Modifier)
                multiplier *= modifier.Value;
        }
        
        return (int) Math.Round(Math.Max(baseValue + flat, 0) * multiplier);
    }
    private int GetBaseStat(UnitStat stat)
    {
        return stat switch
        {
            UnitStat.Power => BaseStats.Power,
            UnitStat.Health => BaseStats.Health,
            UnitStat.Stamina => BaseStats.Stamina,
            UnitStat.Steps => BaseStats.Steps,
            UnitStat.Speed => BaseStats.Speed,
            UnitStat.Vision => BaseStats.Vision,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }

}