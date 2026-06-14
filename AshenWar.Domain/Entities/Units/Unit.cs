using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.Events.Units;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Entities.Units;

public sealed class Unit : DomainEntity, IUnit
{
    private readonly List<UnitCondition> _conditions = [];
    private readonly List<Ability> _abilities = [];
    private readonly List<Passive> _passives = [];

    // Identity
    public UnitId Id { get; }
    public UserId Owner { get; }
    public UnitDefinition Definition { get; }
    public UnitDefinitionId DefinitionId => Definition.Id;
    public StatBlock Stats => Definition.BaseStats;
    public string Name => Definition.Name;
    public IReadOnlySet<EntityTag> Tags => Definition.Tags;
    
    // Position
    public HexCoord? Position { get; private set; }
    
    // Runtime resources (current values, not max - max is always GetStat())
    public int CurrentHealth { get; private set; }
    public int CurrentStamina { get; private set; }
    public int CurrentSteps { get; private set; }
    public bool IsAlive => CurrentHealth >= 0;
    
    // Convenience
    public IReadOnlyList<UnitCondition> Conditions => _conditions.AsReadOnly();
    public IReadOnlyList<Passive> Passives => _passives.AsReadOnly();
    public IReadOnlyList<Ability> Abilities => _abilities.AsReadOnly();
    
    // Constructor
    // For instantiation, use Instantiate()
    private Unit(UnitId id, UserId owner, UnitDefinition definition)
    {
        Id = id;
        Owner = owner;
        Definition = definition;
        CurrentHealth = GetFinalStat(StatDefinition.Health);
        CurrentStamina = GetFinalStat(StatDefinition.Stamina);
        CurrentSteps = GetFinalStat(StatDefinition.Steps);
    }

    // For rehydration, use Rehydrate()
    private Unit(UnitId id,
        UserId owner,
        UnitDefinition definition,
        HexCoord? position,
        int health,
        int stamina,
        int steps)
    {
        Id = id;
        Owner = owner;
        Definition = definition;
        Position = position;
        CurrentHealth = health;
        CurrentStamina = stamina;
        CurrentSteps = steps;
    }

    public static Unit Instantiate(UserId owner, UnitDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(definition);
        
        return new Unit(UnitId.New(), owner, definition);
    }

    public static Unit Rehydrate(
        UnitId id,
        UserId owner,
        UnitDefinition definition,
        HexCoord? position,
        int health,
        int stamina,
        int steps,
        IEnumerable<UnitCondition> unitConditions,
        IEnumerable<Ability> abilities,
        IEnumerable<Passive> passives)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(definition);
        
        if (health < 0) throw new DomainException("Unit health cannot be negative.");
        if (stamina < 0) throw new DomainException("Unit stamina cannot be negative.");
        if (steps < 0) throw new DomainException("Unit steps cannot be negative.");
        
        var unit = new Unit(id, owner, definition, position, health, stamina, steps);
        
        // Rehydrate conditions, abilities, and passives - bypass add/apply logic
        unit._conditions.AddRange(unitConditions);
        unit._abilities.AddRange(abilities);
        unit._passives.AddRange(passives);

        return unit;
    }
    
    #region Abilities

    // IAbilityHolder
    public Ability? GetAbilityByDefinitionId(AbilityDefinitionId abilityDefinitionId)
    {
        ArgumentNullException.ThrowIfNull(abilityDefinitionId);
        return _abilities.FirstOrDefault(a => a.Definition.Id == abilityDefinitionId);
    }
    public Ability? GetAbilityById(AbilityId abilityId)
    {
        ArgumentNullException.ThrowIfNull(abilityId);
        return _abilities.FirstOrDefault(a => a.Id == abilityId);
    }
    public Passive? GetPassiveByDefinitionId(PassiveDefinitionId definitionId)
    {
        if (definitionId == null) throw new ArgumentNullException(nameof(definitionId));
        return _passives.FirstOrDefault(p => p.Definition.Id == definitionId);
    }
    
    // IAbilityCommand
    public void AddPassive(Passive passive)
    {
        _passives.Add(passive);
    }
    public void RemovePassive(PassiveId id)
    {
        _passives.RemoveAll(p => p.Id == id);
    }
    public void AddAbility(Ability active)
    {
        _abilities.Add(active);
    }
    public void RemoveAbility(AbilityId id)
    {
        _abilities.RemoveAll(a => a.Id == id);
    }

    #endregion

    #region Conditions

    // IHasConditions
    public UnitCondition? GetConditionById(ConditionId id)
    {
        return _conditions.FirstOrDefault(s => s.Id == id);
    }
    public UnitCondition? GetConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.FirstOrDefault(s => s.Definition.ConditionTags.Overlaps(conditionTags));
    }
    public bool HasAnyConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.Any(c => c.Definition.ConditionTags.Overlaps(conditionTags));
    }

    // ICondition
    public void ApplyCondition(UnitCondition incoming)
    {
        var existing = _conditions.FirstOrDefault(s => s.Definition.Id == incoming.Definition.Id);
        if (existing is null) 
        {
            AddCondition(incoming);
            return;
        }
        
        incoming.Definition.StackingBehavior.Apply(existing, incoming, AddCondition, RemoveCondition);
    }
    public void RemoveCondition(ConditionId conditionId)
    {
        var effect = _conditions.FirstOrDefault(s => s.Id == conditionId);
        if (effect == null) return;
        
        RemoveCondition(effect);
    }
    
    private void AddCondition(ConditionBase condition)
    {
        if (condition is not UnitCondition incoming)
            throw new ArgumentException("Condition must be of type UnitCondition.", nameof(condition));
        
        _conditions.Add(incoming);
        RaiseDomainEvent(new UnitConditionApplied(Id, incoming.Definition.Id));
    }
    private void RemoveCondition(ConditionBase condition)
    {
        if (condition is not UnitCondition incoming)
            throw new ArgumentException("Condition must be of type UnitCondition.", nameof(condition));
        
        _conditions.Remove(incoming);
        RaiseDomainEvent(new UnitConditionRemoved(Id, incoming.Definition.Id));
    }

    #endregion
    
    #region Movement

    public void MoveTo(HexCoord destination)
    {
        if (Position is null)
            throw new DomainException("Unit cannot move without a current position.");
        
        var previous = Position;
        Position = destination;
        RaiseDomainEvent(new UnitMoved(Id,previous, destination));
    }

    #endregion

    #region Resources

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;
        
        CurrentHealth = Math.Max(CurrentHealth - amount, 0);
        RaiseDomainEvent(new UnitDamaged(Id, amount, CurrentHealth));

        if (CurrentHealth <= 0)
            Die();
    }
    public void RestoreHealth(int amount)
    {
        if (!IsAlive) return;

        CurrentHealth = Math.Min(CurrentHealth + amount, GetFinalStat(StatDefinition.Health));
        RaiseDomainEvent(new UnitHealed(Id, amount, CurrentHealth));
    }
    private void Die()
    {
        if (Position is null)
            throw new DomainException("Unit cannot die without a position.");
        
        RaiseDomainEvent(new UnitDied(Id, Position));
    }

    public void OperateStamina(int delta)
    {
        CurrentStamina = Math.Clamp(CurrentStamina + delta, 0, GetFinalStat(StatDefinition.Stamina));
        RaiseDomainEvent(new UnitStaminaChanged(Id, delta, CurrentStamina));
    }
    public void OperateSteps(int delta)
    {
        CurrentSteps = Math.Clamp(CurrentSteps + delta, 0, GetFinalStat(StatDefinition.Steps));
        RaiseDomainEvent(new UnitStepsChanged(Id, delta, CurrentSteps));
    }

    #endregion

    // IHasStats
    public int GetFinalStat(StatDefinition stat)
    {
        return ModifierCalculator.Calculate(Stats.Get(stat), stat, _conditions, Tags);
    }

    internal void PlacedAt(HexCoord position) => Position = position;
    internal void Removed() => Position = null;
}
