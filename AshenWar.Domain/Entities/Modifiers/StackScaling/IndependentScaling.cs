using Domain.Interfaces;

namespace Domain.Entities.Modifiers.StackScaling;

// Value never changes regardless of stacks
public sealed class IndependentScaling : IStackScaling
{
    public float Calculate(float baseValue, float perStackValue, int stacks) => baseValue;
}