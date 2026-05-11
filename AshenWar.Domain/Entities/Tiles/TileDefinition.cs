using System;
using System.Collections.Generic;
using Domain.Entities.Stats;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.Identifiers.Tiles;

namespace Domain.Entities.Tiles;

public class TileDefinition
{
    private readonly List<PassiveAbilityDefinitionId> _passiveAbilities = [];
    private readonly HashSet<EntityTag> _tags = [];
    
    public TileDefinitionId Id { get; private set; }
    public string Name { get; private set;}
    public VisualId VisualId { get; private set; }
    
    public StatBlock Stats { get; private set; }
    public IReadOnlyList<PassiveAbilityDefinitionId> PassiveAbilities => _passiveAbilities;
    public IReadOnlySet<EntityTag> Tags => _tags;
    
    // Constructor
    private TileDefinition(string name, VisualId visualId, StatBlock stats)
    {
        Id = TileDefinitionId.New();
        Name = name;
        VisualId = visualId;
        Stats = stats;
    }
    public static TileDefinition Create(string name, VisualId visualId, StatBlock stats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(visualId);
        ArgumentNullException.ThrowIfNull(stats);

        return new TileDefinition(name, visualId, stats);
    }

    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        
        Name = newName;
    }
    public void ChangeVisualId(VisualId visualId)
    {
        ArgumentNullException.ThrowIfNull(visualId);
        VisualId = visualId;
    }
    public void UpdateStats(StatBlock stats)
    {
        ArgumentNullException.ThrowIfNull(stats);
        Stats = stats;
    }
    
    // Tags
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags() => _tags.Clear();
    
    // Passive abilities
    public void AddPassiveAbility(PassiveAbilityDefinitionId abilityDefinitionId) => _passiveAbilities.Add(abilityDefinitionId);
    public void RemovePassiveAbility(PassiveAbilityDefinitionId abilityDefinitionId) => _passiveAbilities.Remove(abilityDefinitionId);
    public void ClearPassiveAbilities() => _passiveAbilities.Clear();
}