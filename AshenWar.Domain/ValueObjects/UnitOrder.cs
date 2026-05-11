using System.Collections.Generic;
using Domain.ValueObjects.Identifiers.Units;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.ValueObjects;

public sealed record UnitOrder(UnitId UnitId, ActiveAbilityId AbilityId, IReadOnlyList<PhaseInput> PhaseInputs);