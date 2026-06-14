using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Abilities.Passives.Triggers;

public record TriggerRegistration(
    TriggerEntry Entry,
    PassiveTrigger PassiveTrigger,
    ITargetable Owner,
    HexCoord? AnchorPosition = null);