using System.Collections.Generic;
using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Events;

public sealed record DamageDealt(UnitId UnitId, int Damage, IReadOnlySet<EntityTag> SourceTags) : IDomainEvent;