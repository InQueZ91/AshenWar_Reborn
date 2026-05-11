using System;
using System.Collections.Generic;
using Domain.Interfaces.Actions;
using Domain.ValueObjects.Identifiers.Effects;

namespace Domain.Entities.Effects;

public sealed class EffectDefinition(string name)
{
    private readonly List<IActionDefinition> _actions = [];

    public EffectDefinitionId Id { get; } = EffectDefinitionId.New();
    public string Name { get; private set; } = name;
    public IReadOnlyList<IActionDefinition> Actions =>  _actions;
    
    // Constructor
    public static EffectDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Effect name cannot be empty.", nameof(name));
        
        return new EffectDefinition(name);
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