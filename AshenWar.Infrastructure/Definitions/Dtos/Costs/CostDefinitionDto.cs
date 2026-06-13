using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Costs;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Infrastructure.Definitions.Dtos.Costs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StaminaCostDto), "Stamina")]
[JsonDerivedType(typeof(StepsCostDto), "Steps")]
[JsonDerivedType(typeof(OverloadCostDto), "Overload")]
public abstract record CostDefinitionDto
{
    public abstract CostDefinition ToDomain();
}