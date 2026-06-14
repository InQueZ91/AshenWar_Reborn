using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Actions.Definitions.Costs;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Costs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Stamina), "Stamina")]
[JsonDerivedType(typeof(Steps), "Steps")]
[JsonDerivedType(typeof(Overload), "Overload")]
public abstract record CostDefinition
{
    public abstract bool CanAfford(IReadOnlyUnit readOnlyUnit);
    public abstract CostFailureData GetFailureData(IReadOnlyUnit readOnlyUnit); // called only when CanAfford is false
    public abstract ICostActionDefinition SpendAction { get; }

    public sealed record Stamina(int Amount) : CostDefinition
    {
        public override bool CanAfford(IReadOnlyUnit readOnlyUnit)
            => readOnlyUnit.CurrentStamina >= Amount;

        public override CostFailureData GetFailureData(IReadOnlyUnit readOnlyUnit)
            => new("Stamina", Amount, readOnlyUnit.CurrentStamina);

        public override ICostActionDefinition SpendAction => new SpendStamina(Amount);
    }
    
    public sealed record Steps(int Amount) : CostDefinition
    {
        public override bool CanAfford(IReadOnlyUnit readOnlyUnit) => readOnlyUnit.CurrentSteps >= Amount;
    
        public override CostFailureData GetFailureData(IReadOnlyUnit readOnlyUnit) 
            => new("Steps", Amount, readOnlyUnit.CurrentSteps);
        
        public override ICostActionDefinition SpendAction => new SpendSteps(Amount);
    }
    
    public sealed record Overload(int RequiredStacks) : CostDefinition
    {
        public override bool CanAfford(IReadOnlyUnit readOnlyUnit)
            => readOnlyUnit.GetConditionWithTag(ConditionTag.Overload)?.CurrentStacks >= RequiredStacks;

        public override CostFailureData GetFailureData(IReadOnlyUnit readOnlyUnit)
            => new("Overload", RequiredStacks, readOnlyUnit.GetConditionWithTag(ConditionTag.Overload)?.CurrentStacks ?? 0);

        public override ICostActionDefinition SpendAction => new SpendOverloadStacks(RequiredStacks);
    }
}