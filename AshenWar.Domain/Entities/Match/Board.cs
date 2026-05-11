using System.Collections.Generic;
using System.Linq;
using Domain.Entities.Tiles;
using Domain.Entities.Units;
using Domain.Exceptions;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects;

namespace Domain.Entities.Match;

public sealed class Board : IBoardCommand
{
    // Fast lookup
    private readonly Dictionary<HexCoord, Tile> _tiles = new(); 
    private readonly Dictionary<HexCoord, Unit> _units = new();
    
    // Tile queries
    public ITile GetTile(HexCoord position)
    {
        return _tiles.TryGetValue(position, out var tile) ? tile : throw new DomainException($"No tile at {position}");
    }
    public IEnumerable<ITile> GetAllTiles()
    {
        return _tiles.Values;
    }
    public bool TryGetTile(HexCoord position, out ITile tile)
    {
        var found = _tiles.TryGetValue(position, out var t);
        tile = t!;
        return found;
    }
    public IEnumerable<ITile> GetNeighborsTiles(HexCoord position)
    {
        return position.Neighbors()
            .Where(n => _tiles.ContainsKey(n))
            .Select(n => _tiles[n]);
    }

    // Unit queries
    public IUnit? GetUnitAt(HexCoord position)
    {
        return _units.GetValueOrDefault(position);
    }
    public IEnumerable<IUnit> GetAllUnits()
    {
        return _units.Values;
    }

    // Targetable queries
    public IEnumerable<ITargetable> GetAll()
    {
        return _units.Values.Cast<ITargetable>().Concat(_tiles.Values);
    }
    public IEnumerable<ITargetable> GetAt(HexCoord position)
    {
        if (_tiles.TryGetValue(position, out var tile)) yield return tile;
        if (_units.TryGetValue(position, out var unit)) yield return unit;
    }
    public IEnumerable<ITargetable> GetInRange(HexCoord origin, int range)
    {
        return _tiles.Keys
            .Where(pos => pos.DistanceTo(origin) <= range)
            .SelectMany(GetAt);
    }
    public IEnumerable<ITargetable> GetInCone(HexCoord origin, HexCoord direction, int range)
    {
        // direction is the pointed-at position
        // we find all tiles within range whose angle from origin aligns with the direction vector
        var dir = direction - origin;
        return _tiles.Keys
            .Where(pos =>
            {
                var vec = pos - origin;
                var dist = pos.DistanceTo(origin);
                if (dist == 0 || dist > range) return false;
                // dot product in cube coords - same direction if cross is zero and dot is positive
                return vec.Q * dir.R - vec.R * dir.Q == 0 &&
                       vec.Q * dir.Q + vec.R * dir.R > 0;
            })
            .SelectMany(GetAt);
    }
    
    // Mutation
    public void PlaceTile(Tile tile, HexCoord position)
    {
        // Validation
        // Does the position have a tile?
        if (_tiles.ContainsKey(position))
            throw new DomainException($"Tile already exists at {position}");
        
        // Does the position have a unit?
        if (_units.ContainsKey(position))
            throw new DomainException($"Unit exists at {position}");
        
        // Place tile
        if (!_tiles.TryAdd(position, tile))
            throw new DomainException($"Tile already exists at {position}");
    }
    public Unit? RemoveTile(Tile tile)
    {
        if (!_tiles.ContainsValue(tile))
            throw new DomainException($"Tile {tile.Id} does not exist");
        
        if (tile.Position is not { } tilePos)
            throw new DomainException($"Tile {tile.Id} has no position");
        
        // Remove unit from tile
        _units.Remove(tilePos, out var unit);
        
        // Remove tile from board
        _tiles.Remove(tilePos);
        
        return unit;
    }
    
    public void PlaceUnit(Unit unit, HexCoord position)
    {
        if (!_tiles.TryGetValue(position, out var newTile))
            throw new DomainException($"No tile at {position}");

        if (_units.ContainsKey(position))
            throw new DomainException($"Position {position} is already occupied");

        if (unit.Position is { } oldPos)
        {
            _units.Remove(oldPos);
            if (_tiles.TryGetValue(oldPos, out var oldTile))
                oldTile.SetOccupied(false);
        }

        unit.SetPosition(position);
        _units[position] = unit;
        newTile.SetOccupied(true);
    }
    public void RemoveUnit(Unit unit)
    {
        if (!_units.ContainsValue(unit))
            throw new DomainException($"Unit {unit.Id} does not exist");
        
        if (unit.Position is not { } pos)
            throw new DomainException($"Unit {unit.Id} has no position");
        
        _units.Remove(pos);
            
        if (_tiles.TryGetValue(pos, out var tile)) 
            tile.SetOccupied(false);
            
        unit.ClearPosition();
    }
}