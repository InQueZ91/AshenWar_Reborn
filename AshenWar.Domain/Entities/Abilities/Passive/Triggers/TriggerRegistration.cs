using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Entities.Abilities.Passive.Triggers;

public record TriggerRegistration(
    TriggerEntry Entry,
    PassiveTrigger PassiveTrigger,
    ITargetable Owner,
    HexCoord? AnchorPosition = null);