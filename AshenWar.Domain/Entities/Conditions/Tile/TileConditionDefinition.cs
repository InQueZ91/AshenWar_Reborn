using System;
using System.Collections.Generic;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.LocalIdentifiers;

namespace AshenWar.Domain.Entities.Conditions.Tile;

public sealed class TileConditionDefinition : ConditionDefinitionBase
{
    private readonly List<Modifier> _modifiers = [];
    
    public TileConditionDefinitionId Id { get; }
    
    public IReadOnlyList<Modifier> Modifiers => _modifiers;
    
    // Constructor
    private TileConditionDefinition(TileConditionDefinitionId id, string name) : base(name) => Id = id;
    public static TileConditionDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));

        return new TileConditionDefinition(TileConditionDefinitionId.New(), name);
    }
    public static TileConditionDefinition Load(TileConditionDefinitionId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));

        return new TileConditionDefinition(id, name);
    }
    
    public override IReadOnlyList<ITargetable> ResolveCandidates(ITargetable? holder, IReadOnlyBoard readOnlyBoard) 
        => holder is IReadOnlyTile tile ? [tile] : [];
    
    // Modifiers
    public void AddModifier(Modifier modifier)
    {
        _modifiers.Add(modifier);
    }
    public void RemoveModifier(ModifierDefinitionId modifierDefinition)
    {
        _modifiers.RemoveAll(m => m.Id == modifierDefinition);
    }
    public void ClearModifiers() => _modifiers.Clear();
}