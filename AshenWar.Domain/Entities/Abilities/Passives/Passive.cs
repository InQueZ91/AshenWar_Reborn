using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Abilities.Passives.Triggers;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Entities.Abilities.Passives;

public sealed class Passive
{
    private readonly List<TriggerEntry> _triggers = [];
    
    public PassiveId Id { get; private set;}
    public PassiveDefinition Definition { get; }
    
    public IReadOnlyList<TriggerEntry> Triggers => _triggers;

    private Passive(PassiveId id, PassiveDefinition definition)
    {
        Id = id;
        Definition = definition;
        LoadActiveTriggers();
    }
    public static Passive Instantiate(PassiveDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        
        return new Passive(PassiveId.New(), definition);
    }
    public static Passive Rehydrate(PassiveId id, PassiveDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(definition);

        return new Passive(id, definition);
    }

    private void LoadActiveTriggers()
    {
        Definition.PassiveTriggers
            .Where(eg => eg.IsTriggered)
            .SelectMany(eg => eg.Triggers).ToList()
            .ForEach(t => _triggers.Add(t));
    }
}