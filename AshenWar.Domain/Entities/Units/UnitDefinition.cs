using System;
using System.Collections.Generic;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Entities.Units;

public sealed class UnitDefinition
{
    private readonly List<AbilityDefinitionId> _abilities = [];
    private readonly List<PassiveDefinitionId> _passives = [];
    private readonly HashSet<EntityTag> _tags = [];
    
    // Identifiers
    public UnitDefinitionId Id { get; private set; }
    public string Name { get; private set; }
    public VisualId VisualId { get; private set; }
    public UnitStats BaseStats { get; private set; }
    
    public IReadOnlyList<AbilityDefinitionId> Abilities => _abilities;
    public IReadOnlyList<PassiveDefinitionId> Passives => _passives;
    public IReadOnlySet<EntityTag> Tags => _tags;

    // Constructor
    private UnitDefinition(UnitDefinitionId id, string name, VisualId visualId, UnitStats unitStats)
    {
        Id = id;
        Name = name;
        VisualId = visualId;
        BaseStats = unitStats;
    } 
    public static UnitDefinition Create(VisualId visualId, string name, UnitStats unitStats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(visualId);
        
        return new UnitDefinition(UnitDefinitionId.New(), name, visualId, unitStats);
    }
    public static UnitDefinition Load(UnitDefinitionId id, VisualId visualId, string name, UnitStats unitStats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(visualId);
        ArgumentNullException.ThrowIfNull(unitStats);
        
        return new UnitDefinition(id, name, visualId, unitStats);
    }
    
    // Tag methods
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags() => _tags.Clear();
    
    // Passive ability methods
    public void AddPassive(PassiveDefinitionId definitionId) => _passives.Add(definitionId);
    public void RemovePassive(PassiveDefinitionId definitionId) => _passives.Remove(definitionId);
    public void ClearPassives() => _passives.Clear();
    
    // Active ability methods
    public void AddAbility(AbilityDefinitionId abilityDefinitionId) => _abilities.Add(abilityDefinitionId);
    public void RemoveAbility(AbilityDefinitionId abilityDefinitionId) => _abilities.Remove(abilityDefinitionId);
    public void ClearAbilities() => _abilities.Clear();
    
    // General methods
    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
    }
    public void ChangeVisual(VisualId visualId)
    {
        ArgumentNullException.ThrowIfNull(visualId);
        VisualId = visualId;
    }
    public void SetStats(UnitStats newStats)
    {
        ArgumentNullException.ThrowIfNull(newStats);
        BaseStats = newStats;
    }
}