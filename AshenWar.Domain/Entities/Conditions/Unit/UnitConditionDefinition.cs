using System;
using System.Collections.Generic;
using Domain.Entities.Modifiers;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects.Identifiers.Conditions;
using Domain.ValueObjects.LocalIdentifiers;

namespace Domain.Entities.Conditions.Unit;

public sealed class UnitConditionDefinition(string name) : ConditionDefinitionBase(name)
{
    private readonly List<ModifierDefinition> _modifierDefinitions = [];
    
    public UnitConditionDefinitionId Id { get; } = UnitConditionDefinitionId.New();
    
    public IReadOnlyList<ModifierDefinition> ModifierDefinitions => _modifierDefinitions;
    
    // Constructor
    public static UnitConditionDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));
            
        return new UnitConditionDefinition(name);
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