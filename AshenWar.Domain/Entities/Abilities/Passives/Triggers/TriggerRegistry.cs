using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Abilities.Passives.Triggers;

public class TriggerRegistry : ITriggerRegistry
{
    private readonly List<TriggerRegistration> _registrations = [];

    public void Register(ITargetable owner, Passive passive, HexCoord? anchorPosition = null)
    {
        foreach (var trigger in passive.Definition.PassiveTriggers)
        foreach (var entry in trigger.Triggers)
            _registrations.Add(new TriggerRegistration(entry, trigger, owner, anchorPosition));
    }

    public void Unregister(ITargetable owner, Passive passive)
    {
        var triggerIds = passive.Definition.PassiveTriggers
            .Select(t => t.Id)
            .ToHashSet();

        _registrations.RemoveAll(r => r.Owner == owner && triggerIds.Contains(r.PassiveTrigger.Id));
    }

    public void UnregisterAll(ITargetable owner)
    {
        _registrations.RemoveAll(r => r.Owner == owner);
    }

    public IReadOnlyList<TriggerRegistration> GetMatching(IDomainEvent domainEvent, IBoard board)
    {
        return _registrations
            .Where(r => r.Entry.Matches(domainEvent, r.Owner, board))
            .ToList();
    }
}