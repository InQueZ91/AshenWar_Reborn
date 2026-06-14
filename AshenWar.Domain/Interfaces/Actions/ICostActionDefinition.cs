using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Actions.Definitions.Costs;

namespace AshenWar.Domain.Interfaces.Actions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SpendStamina), "SpendStamina")]
[JsonDerivedType(typeof(SpendSteps), "SpendSteps")]
[JsonDerivedType(typeof(SpendOverloadStacks), "SpendOverloadStacks")]
public interface ICostActionDefinition : IActionDefinition;