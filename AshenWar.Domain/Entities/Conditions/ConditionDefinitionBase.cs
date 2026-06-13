using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Stacking;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Conditions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(GlobalConditionDefinition), "Global")]
[JsonDerivedType(typeof(TileConditionDefinition), "Tile")]
[JsonDerivedType(typeof(UnitConditionDefinition), "Unit")]
public abstract class ConditionDefinitionBase(string name)
{
    private readonly HashSet<EntityTag> _tags = [];
    private readonly HashSet<ConditionTag> _conditionTags = []; // hashset for fast lookup, uniqueness is guaranteed by the enum
    private readonly List<ConditionEffect> _onApply = [];
    private readonly List<ConditionEffect> _onTick = [];
    private readonly List<ConditionEffect> _onExpire = [];
        
    public string Name { get; protected set;} = name;
    public int BaseDuration { get; private set;}
    public IStackingBehavior StackingBehavior { get; private set; } = new NoneStacking();
    public int MaxStacks { get; private set;}
    public ITargetFilter? Filter { get; private set;}
    public IValidator? Guard { get; private set;}
    
    public IReadOnlySet<ConditionTag> ConditionTags => _conditionTags;
    public IReadOnlySet<EntityTag> Tags => _tags;
    public IReadOnlyList<ConditionEffect> OnApply => _onApply;
    public IReadOnlyList<ConditionEffect> OnTick => _onTick;
    public IReadOnlyList<ConditionEffect> OnExpire => _onExpire;
    
    // General methods
    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        
        Name = newName;
    }
    public void SetStackingBehavior(IStackingBehavior rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        StackingBehavior = rule;
    }
    public void SetBaseDuration(int duration)
    {
        if (duration < 1)
            throw new ArgumentException("Duration must be at least 1", nameof(duration));
        BaseDuration = duration;
    }
    public void SetMaxStacks(int maxStacks)
    {
        if (maxStacks < 1)
            throw new ArgumentException("MaxStacks must be at least 1", nameof(maxStacks));
        MaxStacks = maxStacks;
    }

    // Baked in - each subclass defines its own candidate strategy
    public abstract IReadOnlyList<ITargetable> ResolveCandidates(ITargetable? holder, IReadOnlyBoard readOnlyBoard);
    
    // Filter and guard
    public void SetFilter(ITargetFilter filter) => Filter = filter;
    public void SetGuard(IValidator guard) => Guard = guard;
    
    // Entity tags
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags(EntityTag tag) => _tags.Clear();
    
    // Condition tags
    public void AddConditionTag(ConditionTag tag) => _conditionTags.Add(tag);
    public void RemoveConditionTag(ConditionTag tag) => _conditionTags.Remove(tag);
    public void ClearConditionTags() => _conditionTags.Clear();
    
    // Condition effect groups
    public void AddOnApply(ConditionEffect effect) => _onApply.Add(effect);
    public void RemoveOnApply(ConditionEffect effect) => _onApply.Remove(effect);
    public void ClearOnApply() => _onApply.Clear();
    
    public void AddOnTick(ConditionEffect effect) => _onTick.Add(effect);
    public void RemoveOnTick(ConditionEffect effect) => _onTick.Remove(effect);
    public void ClearOnTick() => _onTick.Clear();
    
    public void AddOnExpire(ConditionEffect effect) => _onExpire.Add(effect);
    public void RemoveOnExpire(ConditionEffect effect) => _onExpire.Remove(effect);
    public void ClearOnExpire() => _onExpire.Clear();
}