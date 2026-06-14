using System.Collections.Generic;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Application.Events;

public record AbilityFailed(
    UnitId UnitId,
    AbilityDefinitionId AbilityDefinitionId,
    IReadOnlyList<AffordabilityFailure> Failures);