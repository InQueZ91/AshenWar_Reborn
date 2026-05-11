using System.Collections.Generic;
using System.Linq;
using Domain.Entities.Abilities.Passive.Triggers;
using Domain.Entities.Units;
using Domain.Interfaces;
using Domain.Interfaces.Abilities.Passive;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Entities.Abilities.Passive;

public sealed class PassiveAbility
{
    private readonly List<TriggerEntry> _triggers = [];
    public PassiveAbilityId Id { get; } = PassiveAbilityId.New();
    public PassiveAbilityDefinition Definition { get; }
    
    public IReadOnlyList<TriggerEntry> Triggers => _triggers;

    private PassiveAbility(PassiveAbilityDefinition definition)
    {
        Definition = definition;
        
        // Add all triggers from the passive ability definition
        definition.PassiveTriggers
            .Where(eg => eg.IsTriggered)
            .SelectMany(eg => eg.Triggers).ToList()
            .ForEach(t => _triggers.Add(t));
    }

    public static PassiveAbility Instantiate(PassiveAbilityDefinition definition)
        => new(definition);
}