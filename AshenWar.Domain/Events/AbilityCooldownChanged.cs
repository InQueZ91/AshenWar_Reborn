using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Events;

public sealed record AbilityCooldownChanged(AbilityId AbilityId, int RemainingCooldown) : IDomainEvent;