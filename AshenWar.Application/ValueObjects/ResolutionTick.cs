using System.Collections.Generic;
using Domain.Interfaces;

namespace Application.ValueObjects;

public sealed record ResolutionTick(int TickNumber, IReadOnlyList<IDomainEvent> RecordedEvents);