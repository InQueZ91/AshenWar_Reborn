using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Abilities.Validators;
using AshenWar.Domain.Entities.Abilities.Validators.Primitives;
using AshenWar.Domain.Entities.Actions;

namespace AshenWar.Domain.Interfaces.Abilities;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SourceIsAliveValidator), "SourceIsAlive")]
[JsonDerivedType(typeof(SourceHasStatValidator), "SourceHasStat")]
[JsonDerivedType(typeof(TurnNumberModValidator), "TurnNumberMod")]
[JsonDerivedType(typeof(AndValidator), "And")]
[JsonDerivedType(typeof(OrValidator), "Or")]
[JsonDerivedType(typeof(NotValidator), "Not")]
public interface IValidator
{
    bool Check(ActionContext context);
}