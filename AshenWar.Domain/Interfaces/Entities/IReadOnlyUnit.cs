using System.Collections.Generic;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Interfaces.Entities;

public interface IReadOnlyUnit : 
    ITargetable,
    IPlayerOwned,
    IHasStats,
    IHasPassives,
    IHasAbilities,
    IHasConditions<UnitCondition>
{
    UnitId Id { get; }
    UnitDefinitionId DefinitionId { get; }
    string Name { get; }
    IReadOnlySet<EntityTag> Tags { get; }
    bool IsAlive { get; }
    int CurrentHealth { get; }
    int CurrentStamina { get; }
    int CurrentSteps { get; }
}