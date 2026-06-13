using System.Collections.Generic;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Interfaces.Entities;

public interface IReadOnlyTile : 
    ITargetable,
    IHasStats,
    IHasConditions<TileCondition>,
    IHasPassives
{
    TileId Id { get; }
    TileDefinitionId DefinitionId { get; }
    IReadOnlySet<EntityTag> Tags { get; }
    bool IsClaimed { get; }
    bool HasFog { get; }
    bool IsDestroyed { get; }
}