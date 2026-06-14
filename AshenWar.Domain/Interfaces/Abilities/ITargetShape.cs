using System.Collections.Generic;
using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Abilities.TargetShapes;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Interfaces.Abilities;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Circle), "Circle")]
[JsonDerivedType(typeof(Cone), "Cone")]
[JsonDerivedType(typeof(Self), "Self")]
[JsonDerivedType(typeof(Single), "Single")]
public interface ITargetShape
{
    bool RequiresInput { get; }
    IEnumerable<ITargetable> Resolve(HexCoord origin, ITargetable input, IReadOnlyBoard readOnlyBoard);
}