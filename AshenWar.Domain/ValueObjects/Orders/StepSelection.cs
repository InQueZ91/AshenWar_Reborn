using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.ValueObjects.Orders;

public sealed record StepSelection(AbilityStepId AbilityStepId, Target Target);