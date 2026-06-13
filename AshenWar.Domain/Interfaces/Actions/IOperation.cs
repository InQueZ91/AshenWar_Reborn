using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Modifiers.Operations;

namespace AshenWar.Domain.Interfaces.Actions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Add), "Add")]
[JsonDerivedType(typeof(Subtract), "Subtract")]
[JsonDerivedType(typeof(Multiply), "Multiply")]
[JsonDerivedType(typeof(Divide), "Divide")]
public interface IOperation
{
    int Phase { get; }
    float Apply(float baseValue, float operand);
}