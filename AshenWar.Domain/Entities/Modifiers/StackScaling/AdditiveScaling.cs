using Domain.Interfaces;

namespace Domain.Entities.Modifiers.StackScaling;

public sealed class AdditiveScaling : IStackScaling
{
    public float Calculate(float baseValue, float perStackValue, int stacks) 
        => baseValue + (perStackValue * stacks);
}