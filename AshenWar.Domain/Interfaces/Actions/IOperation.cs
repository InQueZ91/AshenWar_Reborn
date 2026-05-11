namespace Domain.Interfaces.Actions;

public interface IOperation
{
    int Phase { get; }
    float Apply(float baseValue, float operand);
}