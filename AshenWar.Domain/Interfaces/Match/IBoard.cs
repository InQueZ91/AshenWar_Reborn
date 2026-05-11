using System.Collections.Generic;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces.Match;

public interface IBoard
{
    // Tile queries
    ITile GetTile(HexCoord position);
    IEnumerable<ITile> GetAllTiles();
    bool TryGetTile(HexCoord position, out ITile tile);
    IEnumerable<ITile> GetNeighborsTiles(HexCoord position);
    
    // Unit queries
    IUnit? GetUnitAt(HexCoord position);
    IEnumerable<IUnit> GetAllUnits();
    
    // Targetable queries - used by shapes
    IEnumerable<ITargetable> GetAll();
    IEnumerable<ITargetable> GetAt(HexCoord position);
    IEnumerable<ITargetable> GetInRange(HexCoord origin, int range);
    IEnumerable<ITargetable> GetInCone(HexCoord origin, HexCoord target, int range);
}