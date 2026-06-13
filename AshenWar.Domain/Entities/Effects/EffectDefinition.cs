using System;
using System.Collections.Generic;
using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;

namespace AshenWar.Domain.Entities.Effects;

public sealed class EffectDefinition
{
    private readonly List<IActionDefinition> _actions = [];
    
    public EffectDefinitionId Id { get; }
    public string Name { get; private set; }
    public IReadOnlyList<IActionDefinition> Actions =>  _actions;
    
    // Constructor
    private EffectDefinition(EffectDefinitionId id, string name)
    {
        Id = id;
        Name = name;
    }
    public static EffectDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Effect name cannot be empty.", nameof(name));
        
        return new EffectDefinition(EffectDefinitionId.New(), name);
    }
    public static EffectDefinition Load(EffectDefinitionId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Effect name cannot be empty.", nameof(name));
        
        return new EffectDefinition(id, name);
    }
    
    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Effect name cannot be empty.", nameof(name));
        
        Name = name;
    }
    
    // Action methods
    public void AddAction(IActionDefinition action)
    { 
        ArgumentNullException.ThrowIfNull(action);
        _actions.Add(action);
    }
    public void RemoveAction(IActionDefinition action)
    {
        _actions.Remove(action);
    }
    public void ClearActions()
    {
        _actions.Clear();
    }
}