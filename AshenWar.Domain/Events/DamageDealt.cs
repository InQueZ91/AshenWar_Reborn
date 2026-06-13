using System.Collections.Generic;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Events;

public sealed record DamageDealt(UnitId UnitId, int Damage, IReadOnlySet<EntityTag> SourceTags) : IDomainEvent;