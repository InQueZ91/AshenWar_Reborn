using System.Text.Json.Serialization;
using AshenWar.Domain.Entities.Actions.Definitions.Ability;
using AshenWar.Domain.Entities.Actions.Definitions.Ability.Cooldown;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;
using AshenWar.Domain.Entities.Actions.Definitions.Costs;
using AshenWar.Domain.Entities.Actions.Definitions.Movement;
using AshenWar.Domain.Entities.Actions.Definitions.Resource;
using AshenWar.Domain.Entities.Actions.Definitions.Tile;
using AshenWar.Domain.Entities.Actions.Definitions.Unit;

namespace AshenWar.Domain.Interfaces.Actions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SpawnUnit), "SpawnUnit")]

[JsonDerivedType(typeof(SpawnTile), "SpawnTile")]
[JsonDerivedType(typeof(ApplyFog), "ApplyFog")]
[JsonDerivedType(typeof(RemoveFog), "RemoveFog")]
[JsonDerivedType(typeof(DestroyTile), "DestroyTile")]

[JsonDerivedType(typeof(DealDamage), "DealDamage")]
[JsonDerivedType(typeof(Heal), "Heal")]
[JsonDerivedType(typeof(OperateStamina), "OperateStamina")]
[JsonDerivedType(typeof(OperateSteps), "OperateSteps")]

[JsonDerivedType(typeof(Move), "Move")]

[JsonDerivedType(typeof(SpendStamina), "SpendStamina")]
[JsonDerivedType(typeof(SpendSteps), "SpendSteps")]
[JsonDerivedType(typeof(SpendOverloadStacks), "SpendOverloadStacks")]

[JsonDerivedType(typeof(ApplyGlobalCondition), "ApplyGlobalCondition")]
[JsonDerivedType(typeof(ApplyTileCondition), "ApplyTileCondition")]
[JsonDerivedType(typeof(ApplyUnitCondition), "ApplyUnitCondition")]
[JsonDerivedType(typeof(RemoveGlobalCondition), "RemoveGlobalCondition")]
[JsonDerivedType(typeof(RemoveTileCondition), "RemoveTileCondition")]
[JsonDerivedType(typeof(RemoveUnitCondition), "RemoveUnitCondition")]

[JsonDerivedType(typeof(ReduceAbilityCooldown), "ReduceAbilityCooldown")]
[JsonDerivedType(typeof(ReduceAllAbilitiesCooldown), "ReduceAllAbilitiesCooldown")]
[JsonDerivedType(typeof(ResetAbilityCooldown), "ResetAbilityCooldown")]
[JsonDerivedType(typeof(ResetAllAbilitiesCooldown), "ResetAllAbilitiesCooldown")]
[JsonDerivedType(typeof(SetAbilityCooldown), "SetAbilityCooldown")]
[JsonDerivedType(typeof(SetAllAbilitiesCooldown), "SetAllAbilitiesCooldown")]
[JsonDerivedType(typeof(StartAbilityCooldown), "StartAbilityCooldown")]
[JsonDerivedType(typeof(StartAllAbilitiesCooldown), "StartAllAbilitiesCooldown")]
[JsonDerivedType(typeof(GrantActive), "GrantActive")]
[JsonDerivedType(typeof(GrantPassive), "GrantPassive")]
[JsonDerivedType(typeof(RemoveActive), "RemoveActive")]
[JsonDerivedType(typeof(RemovePassive), "RemovePassive")]
public interface IActionDefinition;