using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities.Abilities.Passive;
using Domain.Entities.Conditions;
using Domain.Entities.Conditions.Tile;
using Domain.Entities.Modifiers;
using Domain.Entities.Stats;
using Domain.Enums.Conditions;
using Domain.Events.Tiles;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.Identifiers.Tiles;
using Domain.ValueObjects.LocalIdentifiers.Abilities;
using Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace Domain.Entities.Tiles;

public sealed class Tile : MatchEntity, ITileCommand
{
    private readonly List<TileCondition> _conditions = [];
    private readonly List<PassiveAbility> _passiveAbilities = [];
    
    public TileId Id { get; } = TileId.New();
    public TileDefinition Definition { get; }
    
    // Runtime state
    public HexCoord? Position { get; private set;}
    public bool IsOccupied { get; private set;}
    public bool HasFog { get; private set;}
    public bool IsDestroyed { get; private set;}
    
    public StatBlock BaseStats => Definition.Stats;
    public IReadOnlyList<TileCondition> Conditions => _conditions.AsReadOnly();
    public IReadOnlyList<PassiveAbility> PassiveAbilities => _passiveAbilities.AsReadOnly();
    
    private Tile(TileDefinition definition) => Definition = definition;
    public static Tile Instantiate(TileDefinition definition) => new(definition);

    private void ThrowIfDestroyed()
    {
        if (IsDestroyed)
            throw new InvalidOperationException($"Tile at {Position} is destroyed.");
    }
    public void AddFog()
    {
        ThrowIfDestroyed();
        if (HasFog) return;
        HasFog = true;
        
        // Raise event
    }
    public void RemoveFog()
    {
        ThrowIfDestroyed();
        if (!HasFog) return;
        HasFog = false;
        // Raise event
    }
    public void Destroy()
    {
        ThrowIfDestroyed();
        IsDestroyed = true;
        // Raise event
    }
    internal void SetOccupied(bool isOccupied)
    {
        ThrowIfDestroyed();
        IsOccupied = isOccupied;
        // Raise event
    }

    // IStatHolder
    public int GetMaxStat(StatDefinition stat) 
        => ModifierCalculator.Calculate(BaseStats.Get(stat), _conditions, stat.Id, Definition.Tags);

    // IPassiveAbilityHolder
    public PassiveAbility? GetPassiveByDefinitionId(PassiveAbilityDefinitionId abilityDefinitionId)
    {
        ArgumentNullException.ThrowIfNull(abilityDefinitionId);
        return _passiveAbilities.FirstOrDefault(p => p.Definition.Id == abilityDefinitionId);
    }

    // IPassiveAbilityCommand
    public void AddPassive(PassiveAbility ability)
    {
        _passiveAbilities.Add(ability);
    }
    public void RemovePassive(PassiveAbilityId passiveAbilityId)
    {
        _passiveAbilities.RemoveAll(p => p.Id == passiveAbilityId);
    }

    // IConditionHolder
    public TileCondition? GetConditionById(ConditionId id)
    {
        return _conditions.FirstOrDefault(c => c.Id == id);
    }
    public TileCondition? GetConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.FirstOrDefault(c => c.Definition.ConditionTags.Overlaps(conditionTags));
    }
    public bool HasAnyConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.Any(c => c.Definition.ConditionTags.Overlaps(conditionTags));
    }

    // IConditionCommand
    public void ApplyCondition(TileCondition incoming)
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
        if (condition is not TileCondition incoming)
            throw new ArgumentException("Condition must be of type UnitCondition.", nameof(condition));
        
        _conditions.Add(incoming);
        RaiseDomainEvent(new TileConditionApplied(Id, incoming.Definition.Id));
    }
    private void RemoveCondition(ConditionBase condition)
    {
        if (condition is not TileCondition incoming)
            throw new ArgumentException("Condition must be of type UnitCondition.", nameof(condition));
        
        _conditions.Remove(incoming);
        RaiseDomainEvent(new TileConditionRemoved(Id, incoming.Definition.Id));
    }
    
    internal void SetPosition(HexCoord position) => Position = position;
}