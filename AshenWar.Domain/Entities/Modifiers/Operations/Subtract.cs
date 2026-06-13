using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Modifiers.Operations;

public class Subtract : IOperation
{
    public int Phase => 0;
    public float Apply(float baseValue, float operand) => baseValue - operand;
}