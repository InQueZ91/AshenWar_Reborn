using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Domain.Interfaces.Entities;

public interface ITile : IReadOnlyTile, ICondition<TileCondition>, IPassive
{
    void AddFog();
    void RemoveFog();
    void Destroy();
}