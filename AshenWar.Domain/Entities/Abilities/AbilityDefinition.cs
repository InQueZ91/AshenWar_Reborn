using System;
using System.Collections.Generic;
using AshenWar.Domain.Entities.Costs;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Entities.Abilities;

public sealed class AbilityDefinition
{
    private readonly List<AbilityStep> _steps = [];
    private readonly List<CostDefinition> _costs = [];
    private readonly HashSet<EntityTag> _tags = [];
    
    // Identifier
    public AbilityDefinitionId Id { get; }
    public string Name { get; private set;}
    
    // Properties
    public AbilityStats Stats { get; private set; }
    public IReadOnlyList<CostDefinition> Costs => _costs;
    public IReadOnlyList<AbilityStep> Steps => _steps;
    public IReadOnlySet<EntityTag> Tags => _tags;

    // Constructor
    private AbilityDefinition(AbilityDefinitionId id, string name, AbilityStats stats)
    {
        Id = id;
        Name = name;
        Stats = stats;
    }
    public static AbilityDefinition Create(string name, AbilityStats stats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(stats);
        
        return new AbilityDefinition(AbilityDefinitionId.New(), name, stats);
    }
    public static AbilityDefinition Load(AbilityDefinitionId id, string name, AbilityStats stats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(stats);
        
        return new AbilityDefinition(id, name, stats);
    }

    // General methods
    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
    }
    public void SetStats(AbilityStats newStats)
    {
        ArgumentNullException.ThrowIfNull(newStats);
        Stats = newStats;
    }

    // Tag methods
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags() => _tags.Clear();
    
    // Cost methods
    public void AddCost(CostDefinition cost)
    {
        ArgumentNullException.ThrowIfNull(cost);
        _costs.Add(cost);
    }
    public void RemoveCost(CostDefinition cost)
    {
        ArgumentNullException.ThrowIfNull(cost);
        _costs.Remove(cost);
    }
    public void ClearCosts()
    {
        _costs.Clear();
    }
    
    // Phase methods
    public void AddStep(AbilityStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        _steps.Add(step);
    }
    public void InsertStep(AbilityStep step, int index)
    {
        ArgumentNullException.ThrowIfNull(step);
        _steps.Insert(index, step);
    }
    public void RemoveStep(AbilityStepId id)
    {
        _steps.RemoveAll(p => p.Id == id);
    }
    public void ClearSteps() => _steps.Clear();
}
