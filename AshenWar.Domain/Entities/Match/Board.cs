using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Entities.Match;

public sealed class Board : IBoard
{
    private readonly List<Unit> _units = [];
    private readonly List<Tile> _tiles = [];
    
    // Tile queries
    public IReadOnlyTile GetTileAt(HexCoord position)
    {
        return _tiles.FirstOrDefault(t => t.Position == position) ?? throw new DomainException($"No tile at {position}");
    }
    public ITile GetTileById(TileId tileId)
    {
        return _tiles.FirstOrDefault(t => t.Id == tileId) 
               ?? throw new DomainException($"No tile with id {tileId}");
    }
    public ITile? FindTileById(TileId tileId)
    {
        return _tiles.FirstOrDefault(t => t.Id == tileId);
    }
    public IEnumerable<ITile> GetAllTiles() => _tiles;
    public IEnumerable<IReadOnlyTile> GetNeighborsTiles(HexCoord position)
    {
        var neighbors = position.Neighbors();
        return _tiles.Where(t => t.Position is { } pos && neighbors.Contains(pos));
    }

    // Unit queries
    public IReadOnlyUnit GetUnitAt(HexCoord position)
    {
        return _units.FirstOrDefault(u => u.Position == position) ??
               throw new DomainException($"No unit at {position}");
    }
    public IUnit GetUnitById(UnitId unitId)
    {
        return _units.FirstOrDefault(u => u.Id == unitId) 
               ?? throw new DomainException($"No unit with id {unitId}");
    }
    public IUnit? FindUnitById(UnitId unitId)
    {
        return _units.FirstOrDefault(u => u.Id == unitId);
    }
    public IEnumerable<IUnit> GetAllUnits() => _units;
    public IEnumerable<IReadOnlyUnit> GetUnitsForPlayer(UserId userId) => _units.Where(u => u.Owner == userId).ToList();

    // Targetable queries
    public IEnumerable<ITargetable> GetAll() => _units.Concat<ITargetable>(_tiles);
    public IEnumerable<ITargetable> GetAt(HexCoord position)
    {
        var tile = _tiles.FirstOrDefault(t => t.Position == position);
        if (tile is not null) yield return tile;

        var unit = _units.FirstOrDefault(u => u.Position == position);
        if (unit is not null) yield return unit;
    }
    public IEnumerable<ITargetable> GetInRange(HexCoord origin, int range)
    {
        var tilesInRange = _tiles
            .Where(t => t.Position is { } pos && pos.DistanceTo(origin) <= range);
    
        var unitsInRange = _units
            .Where(u => u.Position is { } pos && pos.DistanceTo(origin) <= range);
    
        return tilesInRange.Concat<ITargetable>(unitsInRange);
    }
    public IEnumerable<ITargetable> GetInCone(HexCoord origin, HexCoord direction, int range)
    {
        // direction is the pointed-at position
        // we find all tiles within range whose angle from origin aligns with the direction vector
        
        var dir = direction - origin;

        var tilesInCone = _tiles.Where(t => InCone(t.Position));
        var unitsInCone = _units.Where(u => InCone(u.Position));
        
        return tilesInCone.Concat<ITargetable>(unitsInCone);

        bool InCone(HexCoord? position)
        {
            if (position is null) return false;
            var vec = position - origin;
            var dist = position.DistanceTo(origin);
            if (dist == 0 || dist > range) return false;
            return vec.Q * dir.R - vec.R * dir.Q == 0 &&
                   vec.Q * dir.Q + vec.R * dir.R > 0;
        }
    }
    
    // Mutation
    public void PlaceTile(Tile tile, HexCoord position)
    {
        // Does position already have tile?
        if (_tiles.FirstOrDefault(t => t.Position == position) is not null)
            throw new DomainException($"Tile already exists at {position}");
        
        // Does position already have unit?
        if (_units.FirstOrDefault(u => u.Position == position) is not null)
            throw new DomainException($"Unit already exists at {position}");
        
        // Does tile already have a position?
        if (tile.Position is not null)
            throw new DomainException($"Tile {tile.Id} already has a position {tile.Position}");

        tile.PlacedAt(position);
        _tiles.Add(tile);
    }
    public void RemoveTile(ITile tile)
    {
        if (!_tiles.Contains(tile))
            throw new DomainException($"Tile {tile.Id} does not exist");
        
        if (tile.Position is not { } tilePos)
            throw new DomainException($"Tile {tile.Id} has no position");
        
        var unit = _units.FirstOrDefault(u => u.Position == tilePos);
        if (unit is not null)
        {
            _units.Remove(unit);
            unit.Removed();
        }
        
        var tileToRemove = _tiles.FirstOrDefault(t => t.Id == tile.Id);
        if (tileToRemove is null)
            throw new DomainException($"Tile {tile.Id} is not in the board");
        
        tileToRemove.Removed();
        _tiles.Remove(tileToRemove);
    }
    
    public void PlaceUnit(Unit unit, HexCoord position)
    {
        var newTile = _tiles.FirstOrDefault(t => t.Position == position)
            ?? throw new DomainException($"No tile at {position}");
        
        if (_units.Any(u => u.Position == position))
            throw new DomainException($"Position {position} is already occupied");

        if (unit.Position is { } oldPos)
        {
            var oldTile = _tiles.FirstOrDefault(t => t.Position == oldPos);
            oldTile?.Release();
        }

        unit.PlacedAt(position);
        _units.Add(unit);
        newTile.Claim();
    }
    public void RemoveUnit(IUnit unit)
    {
        if (!_units.Contains(unit))
            throw new DomainException($"Unit {unit.Id} does not exist");
        
        if (unit.Position is not { } pos)
            throw new DomainException($"Unit {unit.Id} has no position");
        
        var tile = _tiles.FirstOrDefault(t => t.Position == pos);
        tile?.Release();
        
        var unitToRemove = _units.FirstOrDefault(u => u.Id == unit.Id);
        if (unitToRemove is null)
            throw new DomainException($"Unit {unit.Id} is not in the board");
        
        _units.Remove(unitToRemove);
        unitToRemove.Removed();
    }

    internal Board Rehydrate(IEnumerable<Unit> units, IEnumerable<Tile> tiles)
    {
        _units.AddRange(units);
        _tiles.AddRange(tiles);

        return this;
    }
}