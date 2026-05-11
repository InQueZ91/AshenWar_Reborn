using System;
using Domain.Interfaces;

namespace Domain.Entities.Modifiers.StackScaling;

public sealed class MultiplicationScaling : IStackScaling
{
    public float Calculate(float baseValue, float perStackValue, int stacks)
    {
        return baseValue * MathF.Pow(perStackValue, stacks);
    }
}