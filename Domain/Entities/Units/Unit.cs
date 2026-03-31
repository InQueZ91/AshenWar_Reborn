using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Domain.Entities.Abilities;
using Domain.Entities.Effects.Status;
using Domain.Entities.Units.Events;
using Domain.Enums;
using Domain.Interfaces.Effect;
using Domain.Interfaces.Modifier;
using Domain.Interfaces.Object;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Units;

public sealed class Unit : Entity, IEffectTarget, IDamageable, IStatusHolder, IStatHolder<UnitStat>
{
    #region Id and References

    // Identifiers
    public UnitId Id { get; } = UnitId.New();
    public string Name { get; private set; }
    
    // References
    public PlayerId Owner { get; private set; }
    public Vector2 Coord { get; private set; }

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
    
    #endregion

    // State
    public UnitState State { get; private set; }

    public readonly List<StatusEffect> StatusEffects = new List<StatusEffect>();
    private readonly List<Ability> _abilities = new List<Ability>();
    
    #region Constructor

    private Unit(PlayerId owner, Vector2 coord, UnitDefinition definition)
    {
        // References
        Owner = owner;
        Coord = coord;
        
        // Identifiers
        Name = definition.Name;

        // Stats
        BaseStats = definition.BaseStats;
        Power = BaseStats.Power;
        Health = BaseStats.Health;
        Stamina = BaseStats.Stamina;
        Steps = BaseStats.Steps;
        Speed = BaseStats.Speed;
        Vision = BaseStats.Vision;
        
        // Abilities
        foreach (var abilityDef in definition.Abilities)
        {
            _abilities.Add(Ability.Instantiate(abilityDef, this));
        }
    }
    
    public static Unit Instantiate(PlayerId owner, Vector2 coord, UnitDefinition definition)
    {
        return new Unit(owner, coord, definition);
    }

    #endregion
    
    // Methods
    
    #region Effect Methods

    public void AddStatus(StatusEffect statusEffect)
    {
        StatusEffects.Add(statusEffect);
        statusEffect.OnApply(this);
    }
    public void RemoveStatus(Guid statusId)
    {
        var status = StatusEffects.Find(s => s.Id == statusId);

        if (status == null) return;

        StatusEffects.Remove(status);
        status.OnExpire(this);
    }
    public void TickStatusEffects()
    {
        var expiredStatuses = StatusEffects
            .Where(status => status.Tick(this))
            .ToList();

        foreach (var status in expiredStatuses)
        {
            RemoveStatus(status.Id);
        }
    }

    #endregion
    
    #region StatHolder 
    
    public int GetStat(UnitStat stat)
    {
        var modifiers = StatusEffects
            .OfType<IModifierProvider<IUnitStatModifier>>()
            .SelectMany(s => s.GetModifiers())
            .Where(m => m.UnitStat == stat);

        var baseValue = GetBaseStat(stat);
        
        float flat = 0;
        float additive = 0;
        float multiplicative = 1;

        foreach (var m in modifiers)
        {
            switch (m.ModifierType)
            {
                case ModifierType.Flat:
                    flat += m.Modifier;
                    break;
                case ModifierType.Additive:
                    additive += m.Modifier;
                    break;
                case ModifierType.Multiplicative:
                    multiplicative *= m.Modifier;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        var final = (baseValue + flat) * (1 + additive) * multiplicative;
        
        return (int) Math.Round(final);
    }
    public int GetBaseStat(UnitStat stat)
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
        Health = Math.Min(Health + amount, GetStat(UnitStat.Health));

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
        Stamina = Math.Min(Stamina + amount, GetStat(UnitStat.Stamina));

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
        Steps = Math.Min(Steps + amount, GetStat(UnitStat.Steps));

        RaiseDomainEvent(new UnitStepRestored());
    }

    #endregion
}
