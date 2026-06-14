using System;
using System.Collections.Generic;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.LocalIdentifiers;

namespace AshenWar.Domain.Entities.Conditions.Unit;

public sealed class UnitConditionDefinition : ConditionDefinitionBase
{
    private readonly List<Modifier> _modifiers = [];

    public UnitConditionDefinitionId Id { get; }
    
    public IReadOnlyList<Modifier> Modifiers => _modifiers;
    
    // Constructor
    private UnitConditionDefinition(UnitConditionDefinitionId id, string name) : base(name) => Id = id;
    public static UnitConditionDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));
            
        return new UnitConditionDefinition(UnitConditionDefinitionId.New(), name);
    }
    public static UnitConditionDefinition Load(UnitConditionDefinitionId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));   
        
        return new UnitConditionDefinition(id, name);
    }

    public override IReadOnlyList<ITargetable> ResolveCandidates(ITargetable? holder, IReadOnlyBoard readOnlyBoard)
        => holder is IReadOnlyUnit unit ? [unit] : [];
    
    // Modifier definitions
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