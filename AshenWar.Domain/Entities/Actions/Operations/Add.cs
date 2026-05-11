using Domain.Interfaces.Actions;

namespace Domain.Entities.Actions.Operations;

public class Add : IOperation
{
    public int Phase => 0;
    public float Apply(float baseValue, float operand) => baseValue + operand;
}