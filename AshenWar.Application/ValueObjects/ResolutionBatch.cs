using System.Collections.Generic;

namespace AshenWar.Application.ValueObjects;

public sealed record ResolutionBatch(IReadOnlyList<ResolutionTick> ResolutionTicks);