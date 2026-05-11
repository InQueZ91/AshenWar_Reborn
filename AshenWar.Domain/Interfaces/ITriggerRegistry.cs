using System.Collections.Generic;
using Domain.Entities.Abilities.Passive;
using Domain.Entities.Abilities.Passive.Triggers;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces;

public interface ITriggerRegistry
{
    void Register(ITargetable owner, PassiveAbility passiveAbility, HexCoord? anchorPosition = null);
    void Unregister(ITargetable owner, PassiveAbility passiveAbility);
    void UnregisterAll(ITargetable owner);
    IReadOnlyList<TriggerRegistration> GetMatching(IDomainEvent domainEvent);
}