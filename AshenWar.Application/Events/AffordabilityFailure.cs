using Domain.ValueObjects;

namespace Application.Events;

public abstract record AffordabilityFailure;

public sealed record OnCooldown(int RemainingTurns) : AffordabilityFailure;

public sealed record CannotAffordCost(CostFailureData Data) : AffordabilityFailure;