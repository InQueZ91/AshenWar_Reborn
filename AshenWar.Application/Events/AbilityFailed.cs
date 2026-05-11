using System.Collections.Generic;
using Application.Enums;
using Application.Interfaces;
using Domain.ValueObjects.Identifiers.Abilities;
using Domain.ValueObjects.Identifiers.Units;

namespace Application.Events;

public record AbilityFailed(
    UnitId UnitId,
    ActiveAbilityDefinitionId AbilityDefinitionId,
    IReadOnlyList<AffordabilityFailure> Failures);