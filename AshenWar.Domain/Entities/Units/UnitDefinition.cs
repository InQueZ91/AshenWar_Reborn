using System;
using System.Collections.Generic;
using Domain.Entities.Stats;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Entities.Units;

public sealed class UnitDefinition
{
    private readonly List<ActiveAbilityDefinitionId> _activeAbilities = [];
    private readonly List<PassiveAbilityDefinitionId> _passiveAbilities = [];
    private readonly HashSet<EntityTag> _tags = [];
    
    // Identifiers
    public UnitDefinitionId Id { get; private set; }
    public string Name { get; private set; }
    public VisualId VisualId { get; private set; }
    public StatBlock BaseStats { get; private set; }
    
    public IReadOnlyList<ActiveAbilityDefinitionId> ActiveAbilities => _activeAbilities;
    public IReadOnlyList<PassiveAbilityDefinitionId> PassiveAbilities => _passiveAbilities;
    public IReadOnlySet<EntityTag> Tags => _tags;

    // Constructor
    private UnitDefinition(string name, VisualId visualId, StatBlock statBlock)
    {
        Id = UnitDefinitionId.New();
        Name = name;
        VisualId = visualId;
        BaseStats = statBlock;
    } 
    public static UnitDefinition Create(VisualId visualId, string name, StatBlock statBlock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(visualId);
        
        return new UnitDefinition(name, visualId, statBlock);
    }
    
    // Tag methods
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags() => _tags.Clear();
    
    // Passive ability methods
    public void AddPassiveAbility(PassiveAbilityDefinitionId abilityDefinitionId) => _passiveAbilities.Add(abilityDefinitionId);
    public void RemovePassiveAbility(PassiveAbilityDefinitionId abilityDefinitionId) => _passiveAbilities.Remove(abilityDefinitionId);
    public void ClearPassiveAbilities() => _passiveAbilities.Clear();
    
    // Active ability methods
    public void AddActiveAbility(ActiveAbilityDefinitionId abilityDefinitionId) => _activeAbilities.Add(abilityDefinitionId);
    public void RemoveActiveAbility(ActiveAbilityDefinitionId abilityDefinitionId) => _activeAbilities.Remove(abilityDefinitionId);
    public void ClearActiveAbilities() => _activeAbilities.Clear();
    
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
    public void SetBaseStats(StatBlock newStats)
    {
        ArgumentNullException.ThrowIfNull(newStats);
        BaseStats = newStats;
    }
}