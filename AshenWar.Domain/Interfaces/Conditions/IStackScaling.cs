using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Modifiers.StackScaling;

namespace AshenWar.Domain.Interfaces;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AdditiveScaling), "Additive")]
[JsonDerivedType(typeof(IndependentScaling), "Independent")]
[JsonDerivedType(typeof(MultiplicationScaling), "Multiplicative")]
[JsonDerivedType(typeof(OverrideScaling), "Override")]
public interface IStackScaling
{
    float Calculate(float baseValue, float perStackValue, int stacks);
}