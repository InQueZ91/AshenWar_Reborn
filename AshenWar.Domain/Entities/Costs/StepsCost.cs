using Domain.Entities.Actions.Definitions.Costs;
using Domain.Interfaces;
using Domain.Interfaces.Actions;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Entities.Costs;

public sealed record StepsCost(int Amount) : ICostDefinition
{
    public bool CanAfford(IUnit unit) => unit.CurrentSteps >= Amount;
    
    public CostFailureData GetFailureData(IUnit unit) 
        => new("Steps", Amount, unit.CurrentSteps);

    public ICostActionDefinition SpendAction => new SpendSteps(Amount);
}