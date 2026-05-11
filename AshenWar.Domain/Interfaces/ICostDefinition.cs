using Domain.Interfaces.Actions;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces;

public interface ICostDefinition
{
    bool CanAfford(IUnit unit);
    CostFailureData GetFailureData(IUnit unit); // called only when CanAfford is false
    ICostActionDefinition SpendAction { get; }
}