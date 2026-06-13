using System;
using System.Collections.Generic;
using AshenWar.Domain.Entities.Abilities.Passives.Triggers;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Entities.Abilities.Passives;

public sealed class PassiveDefinition
{
    private readonly List<PassiveTrigger> _passiveTriggers = [];
    
    public PassiveDefinitionId Id { get; }
    public string Name { get; private set;}
    
    public IReadOnlyList<PassiveTrigger> PassiveTriggers => _passiveTriggers;
    
    // Constructor
    private PassiveDefinition(PassiveDefinitionId id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public static PassiveDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        return new PassiveDefinition(PassiveDefinitionId.New(), name);
    }
    
    public static PassiveDefinition Load(PassiveDefinitionId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        return new PassiveDefinition(id, name);
    }

    // General methods
    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
    }
    
    public void AddPassiveTrigger(PassiveTrigger group)
    {
        ArgumentNullException.ThrowIfNull(group);
        _passiveTriggers.Add(group);
    }
    public void RemovePassiveTrigger(PassiveTriggerId id)
    {
        _passiveTriggers.RemoveAll(g => g.Id == id);
    }
    public void ClearPassiveTriggers()
    {
        _passiveTriggers.Clear();
    }
}