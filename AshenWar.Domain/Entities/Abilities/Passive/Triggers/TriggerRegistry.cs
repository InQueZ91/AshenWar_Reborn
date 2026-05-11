using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Entities.Abilities.Passive.Triggers;

public class TriggerRegistry : ITriggerRegistry
{
    private readonly List<TriggerRegistration> _registrations = [];

    public void Register(ITargetable owner, PassiveAbility passiveAbility, HexCoord? anchorPosition = null)
    {
        foreach (var trigger in passiveAbility.Definition.PassiveTriggers)
        foreach (var entry in trigger.Triggers)
            _registrations.Add(new TriggerRegistration(entry, trigger, owner, anchorPosition));
    }

    public void Unregister(ITargetable owner, PassiveAbility passiveAbility)
    {
        var triggerIds = passiveAbility.Definition.PassiveTriggers
            .Select(t => t.Id)
            .ToHashSet();

        _registrations.RemoveAll(r => r.Owner == owner && triggerIds.Contains(r.PassiveTrigger.Id));
    }

    public void UnregisterAll(ITargetable owner)
    {
        _registrations.RemoveAll(r => r.Owner == owner);
    }

    public IReadOnlyList<TriggerRegistration> GetMatching(IDomainEvent domainEvent)
    {
        return _registrations
            .Where(r => r.Entry.Matches(domainEvent))
            .ToList();
    }
}