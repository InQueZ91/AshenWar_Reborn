using Domain.Interfaces.Entities;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.ValueObjects;

public sealed record PhaseInput(AbilityPhaseId AbilityPhaseId, ITargetable SelectedTarget);