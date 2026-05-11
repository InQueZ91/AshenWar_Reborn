using Domain.Interfaces;

namespace Domain.Entities.Modifiers.StackScaling;

// Ignore baseValue entirely - pure stack-driven value
public sealed class OverrideScaling : IStackScaling
{
    public float Calculate(float baseValue, float perStackValue, int stacks) => perStackValue * stacks;
}