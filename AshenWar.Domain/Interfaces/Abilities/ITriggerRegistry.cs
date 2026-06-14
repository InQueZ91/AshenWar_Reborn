using System.Collections.Generic;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Abilities.Passives.Triggers;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Interfaces.Abilities;

public interface ITriggerRegistry
{
    void Register(ITargetable owner, Passive passive, HexCoord? anchorPosition = null);
    void Unregister(ITargetable owner, Passive passive);
    void UnregisterAll(ITargetable owner);
    IReadOnlyList<TriggerRegistration> GetMatching(IDomainEvent domainEvent, IBoard board);
}