using System.Collections.Generic;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Application.ValueObjects;

public sealed record ResolutionTick(int TickNumber, IReadOnlyList<ResolutionEvent> Events);