using Domain.Entities.Conditions.Tile;
using Domain.Entities.Tiles;
using Domain.Interfaces.Abilities.Passive;
using Domain.Interfaces.Conditions;
using Domain.ValueObjects.Identifiers.Tiles;

namespace Domain.Interfaces.Entities;

public interface ITile : ITargetable, IStatHolder, IConditionHolder<TileCondition>, IPassiveAbilityHolder
{
    TileId Id { get; }
    bool IsOccupied { get; }
    bool HasFog { get; }
    bool IsDestroyed { get; }
    TileDefinition Definition { get; }
}