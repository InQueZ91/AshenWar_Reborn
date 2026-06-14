using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Modifiers.Operations;

public class Override : IOperation
{
    public int Phase => 2;
    public float Apply(float baseValue, float operand) => operand;
}