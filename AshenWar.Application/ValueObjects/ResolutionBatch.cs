using System.Collections.Generic;
using Domain.ValueObjects.Identifiers.Units;

namespace Application.ValueObjects;

public sealed record ResolutionBatch(IReadOnlyList<ResolutionTick> ResolutionTicks);