using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities.Abilities.Active;
using Domain.Entities.Abilities.Passive;
using Domain.Entities.Conditions;
using Domain.Entities.Conditions.Unit;
using Domain.Entities.Modifiers;
using Domain.Entities.Stats;
using Domain.Enums.Conditions;
using Domain.Events.Units;
using Domain.Exceptions;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;
using Domain.ValueObjects.LocalIdentifiers.Abilities;
using Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace Domain.Entities.Units;

public sealed class Unit : MatchEntity, IUnitCommand
{
    private readonly List<UnitCondition> _conditions = [];
    private readonly List<ActiveAbility> _activeAbilities = [];
    private readonly List<PassiveAbility> _passiveAbilities = [];

    // Identity
    public UnitId Id { get; } = UnitId.New();
    public UserId Owner { get; }
    public UnitDefinition Definition { get; }
    
    // Position
    public HexCoord? Position { get; private set; }
    
    // State
    public bool IsAlive {get; private set;} = true;

    // Runtime resources (current values, not max - max is always GetStat())
    public int CurrentHealth { get; private set; }
    public int CurrentStamina { get; private set; }
    public int CurrentSteps { get; private set; }
    
    // Convenience
    public StatBlock BaseStats => Definition.BaseStats;
    public IReadOnlyList<UnitCondition> Conditions => _conditions.AsReadOnly();
    public IReadOnlyList<PassiveAbility> PassiveAbilities => _passiveAbilities.AsReadOnly();
    public IReadOnlyList<ActiveAbility> ActiveAbilities => _activeAbilities.AsReadOnly();
    
    // Constructor
    private Unit(UserId owner, UnitDefinition definition)
    {
        Owner = owner;
        Definition = definition;
        
        CurrentHealth = GetMaxStat(StatDefinition.Health);
        CurrentStamina = GetMaxStat(StatDefinition.Stamina);
        CurrentSteps = GetMaxStat(StatDefinition.Steps);
    }
    public static Unit Instantiate(UserId owner, UnitDefinition definition)
    {
        return new Unit(owner, definition);
    }
    
    #region Abilities

    // IAbilityHolder
    public ActiveAbility? GetActiveAbilityByDefinitionId(ActiveAbilityDefinitionId abilityDefinitionId)
    {
        ArgumentNullException.ThrowIfNull(abilityDefinitionId);
        return _activeAbilities.FirstOrDefault(a => a.Definition.Id == abilityDefinitionId);
    }
    public ActiveAbility? GetActiveAbilityById(ActiveAbilityId abilityId)
    {
        ArgumentNullException.ThrowIfNull(abilityId);
        return _activeAbilities.FirstOrDefault(a => a.Id == abilityId);
    }
    public PassiveAbility? GetPassiveByDefinitionId(PassiveAbilityDefinitionId abilityDefinitionId)
    {
        if (abilityDefinitionId == null) throw new ArgumentNullException(nameof(abilityDefinitionId));
        return _passiveAbilities.FirstOrDefault(p => p.Definition.Id == abilityDefinitionId);
    }
    
    // IAbilityCommand
    public void AddPassive(PassiveAbility passive)
    {
        _passiveAbilities.Add(passive);
    }
    public void RemovePassive(PassiveAbilityId id)
    {
        _passiveAbilities.RemoveAll(p => p.Id == id);
    }
    public void AddActiveAbility(ActiveAbility active)
    {
        _activeAbilities.Add(active);
    }
    public void RemoveActiveAbility(ActiveAbilityId id)
    {
        _activeAbilities.RemoveAll(a => a.Id == id);
    }

    #endregion

    #region Conditions

    // IConditionHolder
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

    // IConditionCommand
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
    
    // IConditionTarget
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

    // IStatHolder
    public int GetMaxStat(StatDefinition stat) 
        => ModifierCalculator.Calculate(BaseStats.Get(stat), _conditions, stat.Id, Definition.Tags);

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

        CurrentHealth = Math.Min(CurrentHealth + amount, GetMaxStat(StatDefinition.Health));
        RaiseDomainEvent(new UnitHealed(Id, amount, CurrentHealth));
    }
    private void Die()
    {
        if (Position is null)
            throw new DomainException("Unit cannot die without a position.");
        
        IsAlive = false;
        RaiseDomainEvent(new UnitDied(Id, Position));
    }

    public void OperateStamina(int delta)
    {
        CurrentStamina = Math.Clamp(CurrentStamina + delta, 0, GetMaxStat(StatDefinition.Stamina));
        RaiseDomainEvent(new UnitStaminaChanged(Id, delta, CurrentStamina));
    }
    public void OperateSteps(int delta)
    {
        CurrentSteps = Math.Clamp(CurrentSteps + delta, 0, GetMaxStat(StatDefinition.Steps));
        RaiseDomainEvent(new UnitStepsChanged(Id, delta, CurrentSteps));
    }

    #endregion
    
    internal void SetPosition(HexCoord position) => Position = position;
    internal void ClearPosition() => Position = null; // on Remove Unit
}
