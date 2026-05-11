using System.Collections.Generic;
using Domain.Entities.Conditions.Global;
using Domain.Entities.Tiles;
using Domain.Entities.Units;
using Domain.Interfaces.Conditions;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Tiles;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Interfaces.Match;

public interface IMatchCommand : IMatch, IConditionCommand<GlobalCondition>
{
    IBoardCommand Board { get; }
    
    Unit? GetUnitById(UnitId id);
    List<Unit> GetUnitsForPlayer(UserId playerId);
    Tile? GetTileById(TileId tileId);
    
    void SpawnUnit(Unit unit, HexCoord position);
    void DespawnUnit(Unit unit);
    
    void SpawnTile(Tile tile, HexCoord position);
    void DespawnTile(Tile tile);
}