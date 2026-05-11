namespace Domain.Entities.Conditions.Global;

public sealed class GlobalCondition : ConditionBase
{
    public override GlobalConditionDefinition Definition { get; }
    
    private GlobalCondition(GlobalConditionDefinition definition, int stacks) :
        base(definition.BaseDuration, stacks, definition.MaxStacks)
        => Definition = definition;
    
    public static GlobalCondition Instantiate(GlobalConditionDefinition definition, int stacks = 1)
        => new (definition, stacks);
}