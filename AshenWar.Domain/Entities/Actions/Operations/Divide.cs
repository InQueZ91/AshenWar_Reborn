using Domain.Interfaces.Actions;

namespace Domain.Entities.Actions.Operations;

public class Divide : IOperation
{
    public int Phase => 1;
    public float Apply(float baseValue, float operand) => baseValue / operand;
}