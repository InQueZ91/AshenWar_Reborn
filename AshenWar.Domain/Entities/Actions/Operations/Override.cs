using Domain.Interfaces.Actions;

namespace Domain.Entities.Actions.Operations;

public class Override : IOperation
{
    public int Phase => 2;
    public float Apply(float baseValue, float operand) => operand;
}