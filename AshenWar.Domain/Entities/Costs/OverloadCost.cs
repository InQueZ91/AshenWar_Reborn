using Domain.Entities.Actions.Definitions.Costs;
using Domain.Enums.Conditions;
using Domain.Interfaces;
using Domain.Interfaces.Actions;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Entities.Costs;

public sealed record OverloadCost(int RequiredStacks) : ICostDefinition
{
    public bool CanAfford(IUnit unit)
        => unit.GetConditionWithTag(ConditionTag.Overload)?.Stacks >= RequiredStacks;

    public CostFailureData GetFailureData(IUnit unit)
        => new("Overload", RequiredStacks, unit.GetConditionWithTag(ConditionTag.Overload)?.Stacks ?? 0);

    public ICostActionDefinition SpendAction => new SpendOverloadStacks(RequiredStacks);
}