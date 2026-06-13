using System.Collections.Generic;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Interfaces.Match;

public interface IBoard : IReadOnlyBoard
{
    // Tile Management
    IEnumerable<ITile> GetAllTiles();
    ITile GetTileById(TileId tileId);
    ITile? FindTileById(TileId tileId);
    void PlaceTile(Tile tile, HexCoord position); 
    void RemoveTile(ITile tile);
    
    // Unit Management
    IEnumerable<IUnit> GetAllUnits();
    IUnit GetUnitById(UnitId unitId);
    IUnit? FindUnitById(UnitId id);
    void PlaceUnit(Unit unit, HexCoord position);
    void RemoveUnit(IUnit unit);
}