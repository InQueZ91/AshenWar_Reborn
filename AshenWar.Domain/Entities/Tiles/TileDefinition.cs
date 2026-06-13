using System;
using System.Collections.Generic;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Entities.Tiles;

public class TileDefinition
{
    private readonly List<PassiveDefinitionId> _passives = [];
    private readonly HashSet<EntityTag> _tags = [];
    
    public TileDefinitionId Id { get; private set; }
    public string Name { get; private set;}
    public VisualId VisualId { get; private set; }
    
    public TileStats BaseStats { get; private set; }
    public IReadOnlyList<PassiveDefinitionId> Passives => _passives;
    public IReadOnlySet<EntityTag> Tags => _tags;
    
    // Constructor
    private TileDefinition(TileDefinitionId id, string name, VisualId visualId, TileStats baseStats)
    {
        Id = id;
        Name = name;
        VisualId = visualId;
        BaseStats = baseStats;
    }
    public static TileDefinition Create(VisualId visualId, string name, TileStats stats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(visualId);
        ArgumentNullException.ThrowIfNull(stats);

        return new TileDefinition(TileDefinitionId.New(), name, visualId, stats);
    }
    public static TileDefinition Load(TileDefinitionId id, VisualId visualId, string name, TileStats stats)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        ArgumentNullException.ThrowIfNull(visualId);
        ArgumentNullException.ThrowIfNull(stats);

        return new TileDefinition(id, name, visualId, stats);
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
    public void SetStats(TileStats stats)
    {
        ArgumentNullException.ThrowIfNull(stats);
        BaseStats = stats;
    }
    
    // Tags
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags() => _tags.Clear();
    
    // Passive abilities
    public void AddPassive(PassiveDefinitionId definitionId) => _passives.Add(definitionId);
    public void RemovePassive(PassiveDefinitionId definitionId) => _passives.Remove(definitionId);
    public void ClearPassives() => _passives.Clear();
}