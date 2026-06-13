using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Interfaces.Match;

public interface IMatch : IReadOnlyMatch, ICondition<GlobalCondition>
{
    IBoard Board { get; }
    
    void SpawnUnit(Unit unit, HexCoord position);
    void DespawnUnit(IUnit unit);
    
    void SpawnTile(Tile tile, HexCoord position);
    void DespawnTile(ITile tile);
}