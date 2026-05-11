using System;
using System.Collections.Generic;
using Domain.Entities.Abilities.Passive.Triggers;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Entities.Abilities.Passive;

public sealed class PassiveAbilityDefinition
{
    private readonly List<PassiveTrigger> _passiveTriggers = [];
    
    public PassiveAbilityDefinitionId Id { get; } = PassiveAbilityDefinitionId.New(); 
    public string Name { get; private set;}
    
    public IReadOnlyList<PassiveTrigger> PassiveTriggers => _passiveTriggers;
    
    private PassiveAbilityDefinition(string name) => Name = name;
    public static PassiveAbilityDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        return new PassiveAbilityDefinition(name);
    }

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