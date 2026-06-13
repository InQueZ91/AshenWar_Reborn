using System.Collections.Generic;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Domain.Interfaces.Match;

public interface IReadOnlyBoard
{
    // Tile queries
    IReadOnlyTile GetTileAt(HexCoord position);
    IEnumerable<IReadOnlyTile> GetNeighborsTiles(HexCoord position);
    
    // Unit queries
    IReadOnlyUnit GetUnitAt(HexCoord position);
    IEnumerable<IReadOnlyUnit> GetUnitsForPlayer(UserId userId);
    
    // Targetable queries - used by shapes
    IEnumerable<ITargetable> GetAll();
    IEnumerable<ITargetable> GetAt(HexCoord position);
    IEnumerable<ITargetable> GetInRange(HexCoord origin, int range);
    IEnumerable<ITargetable> GetInCone(HexCoord origin, HexCoord target, int range);
}