namespace Domain.Interfaces;

public interface IStackScaling
{
    float Calculate(float baseValue, float perStackValue, int stacks);
}