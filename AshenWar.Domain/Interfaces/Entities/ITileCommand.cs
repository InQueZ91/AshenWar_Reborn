using Domain.Entities.Conditions.Tile;
using Domain.Interfaces.Abilities.Passive;
using Domain.Interfaces.Conditions;

namespace Domain.Interfaces.Entities;

public interface ITileCommand : ITile, IConditionCommand<TileCondition>, IPassiveAbilityCommand
{
    void AddFog();
    void RemoveFog();
    void Destroy();
}