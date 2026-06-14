using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Abilities.Passives.Triggers.Entries;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;

namespace AshenWar.Domain.Entities.Abilities.Passives.Triggers;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(OnUnitDamaged), "OnUnitDamaged")]
public abstract record TriggerEntry
{
    public abstract bool Matches(IDomainEvent evt, ITargetable owner, IBoard board);
}