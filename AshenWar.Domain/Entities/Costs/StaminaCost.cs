using Domain.Entities.Actions.Definitions.Costs;
using Domain.Interfaces;
using Domain.Interfaces.Actions;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Entities.Costs;

public sealed record StaminaCost(int Amount) : ICostDefinition
{
    public bool CanAfford(IUnit unit) => unit.CurrentStamina >= Amount;
    
    public CostFailureData GetFailureData(IUnit unit)
        => new("Stamina", Amount, unit.CurrentStamina);
    
    public ICostActionDefinition SpendAction => new SpendStamina(Amount);
}