using AshenWar.Domain.ValueObjects;

namespace AshenWar.Application.Events;

public abstract record AffordabilityFailure;

public sealed record OnCooldown(int RemainingTurns) : AffordabilityFailure;

public sealed record CannotAffordCost(CostFailureData Data) : AffordabilityFailure;