using System.Text.Json.Serialization;
using Domain.Entities.Actions.Definitions.Ability;
using Domain.Entities.Actions.Definitions.Board;
using Domain.Entities.Actions.Definitions.Condition;
using Domain.Entities.Actions.Definitions.Costs;
using Domain.Entities.Actions.Definitions.Movement;
using Domain.Entities.Actions.Definitions.Resource;
using Domain.Entities.Actions.Definitions.Unit;

namespace Domain.Interfaces.Actions;

/// <summary>
/// Marker interface for all action blueprints.
/// Embedded as owned JSON inside EffectDefinition — no own DB table, no ID.
/// Handler registry in AshenWar.Application layer dispatches on concrete type.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DealDamage),         "dealDamage")]
[JsonDerivedType(typeof(Heal),               "heal")]
[JsonDerivedType(typeof(Move),               "move")]
[JsonDerivedType(typeof(OperateStamina),     "operateStamina")]
[JsonDerivedType(typeof(OperateSteps),       "operateSteps")]
[JsonDerivedType(typeof(ApplyUnitCondition),     "applyCondition")]
[JsonDerivedType(typeof(RemoveUnitCondition),    "removeCondition")]
[JsonDerivedType(typeof(SpawnUnit),          "spawnUnit")]
[JsonDerivedType(typeof(StartAbilityCooldown),      "resetCooldown")]
[JsonDerivedType(typeof(SetCooldown),        "setCooldown")]
[JsonDerivedType(typeof(AddFog),             "addFog")]
[JsonDerivedType(typeof(RemoveFog),          "removeFog")]
[JsonDerivedType(typeof(DestroyTile),        "destroyTile")]
[JsonDerivedType(typeof(SpendStamina),         "deductStamina")]
[JsonDerivedType(typeof(SpendSteps),           "deductSteps")]
[JsonDerivedType(typeof(SpendOverloadStacks),  "deductOverloadStacks")]
public interface IActionDefinition {}