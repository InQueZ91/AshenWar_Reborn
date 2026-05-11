using System;
using System.Collections.Generic;
using Domain.Entities.Stats;
using Domain.Interfaces;
using Domain.Interfaces.Abilities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Entities.Abilities.Active;

public sealed class ActiveAbilityDefinition
{
    private readonly List<AbilityPhase> _phases = [];
    private readonly List<ICostDefinition> _costs = [];
    private readonly HashSet<EntityTag> _tags = [];
    
    // Identifier
    public ActiveAbilityDefinitionId Id { get; } = ActiveAbilityDefinitionId.New();
    public string Name { get; private set;}
    
    // Properties
    public StatBlock Stats { get; private set; }
    public IReadOnlyList<ICostDefinition> Costs => _costs;
    public IReadOnlyList<AbilityPhase> Phases => _phases;
    public IReadOnlySet<EntityTag> Tags => _tags;

    private ActiveAbilityDefinition(string name, StatBlock stats)
    {
        Name = name;
        Stats = stats;
    }
    public static ActiveAbilityDefinition Create(string name, StatBlock stats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(stats);
        
        return new ActiveAbilityDefinition(name, stats);
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
    }
    public void UpdateStats(StatBlock stats)
    {
        ArgumentNullException.ThrowIfNull(stats);
        Stats = stats;
    }

    // Tag methods
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags() => _tags.Clear();
    
    // Cost methods
    public void AddCost(ICostDefinition cost)
    {
        ArgumentNullException.ThrowIfNull(cost);
        _costs.Add(cost);
    }
    public void RemoveCost(ICostDefinition cost)
    {
        ArgumentNullException.ThrowIfNull(cost);
        _costs.Remove(cost);
    }
    public void ClearCosts()
    {
        _costs.Clear();
    }
    
    // Phase methods
    public void AddPhase(AbilityPhase phase)
    {
        ArgumentNullException.ThrowIfNull(phase);
        _phases.Add(phase);
    }
    public void InsertPhase(AbilityPhase phase, int index)
    {
        ArgumentNullException.ThrowIfNull(phase);
        _phases.Insert(index, phase);
    }
    public void RemovePhase(AbilityPhaseId id)
    {
        _phases.RemoveAll(p => p.Id == id);
    }
    public void ClearPhases() => _phases.Clear();
}
