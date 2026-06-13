using System.Collections.Generic;
using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Abilities.TargetFilters;
using AshenWar.Domain.Entities.Abilities.TargetFilters.Primitives;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Interfaces.Abilities;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AlliesOnly), "AlliesOnly")]
[JsonDerivedType(typeof(EmptyTiles), "EmptyTiles")]
[JsonDerivedType(typeof(EnemiesOnly), "EnemiesOnly")]
[JsonDerivedType(typeof(Self), "Self")]
[JsonDerivedType(typeof(TilesOnly), "TilesOnly")]
[JsonDerivedType(typeof(UnitsOnly), "UnitsOnly")]
[JsonDerivedType(typeof(AllOf), "AllOf")]
[JsonDerivedType(typeof(AnyOf), "AnyOf")]
[JsonDerivedType(typeof(NoneOf), "NoneOf")]
public interface ITargetFilter
{
    IEnumerable<ITargetable> Apply(IEnumerable<ITargetable> candidates, ITargetable source);
}