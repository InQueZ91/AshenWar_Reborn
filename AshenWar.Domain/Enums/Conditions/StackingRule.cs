namespace Domain.Enums.Conditions;

// What happens when effect is applied again?
public enum StackingRule
{
    None,
    RefreshDuration,
    StackDuration,
    StackIntensity,
    Replace,
    Ignore
}