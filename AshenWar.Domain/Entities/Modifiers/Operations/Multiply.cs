using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Modifiers.Operations;

public class Multiply : IOperation
{
    public int Phase => 1;
    public float Apply(float baseValue, float operand) => baseValue * operand;
}