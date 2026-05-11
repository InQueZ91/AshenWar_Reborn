using System;
using System.Collections.Generic;
using Domain.Entities.Modifiers;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects.Identifiers.Conditions;
using Domain.ValueObjects.LocalIdentifiers;

namespace Domain.Entities.Conditions.Tile;

public sealed class TileConditionDefinition(string name) : ConditionDefinitionBase(name)
{
    private readonly List<ModifierDefinition> _modifierDefinitions = [];
    
    public TileConditionDefinitionId Id { get; } = TileConditionDefinitionId.New();
    
    public IReadOnlyList<ModifierDefinition> ModifierDefinitions => _modifierDefinitions;
    
    public static TileConditionDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));

        return new TileConditionDefinition(name);
    }
    
    public override IReadOnlyList<ITargetable> ResolveCandidates(ITargetable? holder, IBoard board) => [holder!];
    
    // Modifier definitions
    public void AddModifierDefinition(ModifierDefinition modifierDefinition)
    {
        _modifierDefinitions.Add(modifierDefinition);
    }
    public void RemoveModifierDefinition(ModifierDefinitionId modifierDefinition)
    {
        _modifierDefinitions.RemoveAll(m => m.Id == modifierDefinition);
    }
    public void ClearModifierDefinitions() => _modifierDefinitions.Clear();
}