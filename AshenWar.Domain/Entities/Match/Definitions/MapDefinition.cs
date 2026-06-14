using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Entities.Match.Definitions;

public sealed class MapDefinition
{
    private readonly Dictionary<HexCoord, TileDefinitionId?> _tiles = new();
    private readonly Dictionary<PlayerSide, List<HexCoord>> _deploymentPoints = new();
    private readonly List<GlobalConditionDefinitionId> _initialGlobalConditions = [];
    private readonly List<GlobalEvent> _globalEvents = [];

    public MapDefinitionId Id { get; private set; }
    public string Name { get; private set;}
    public int Radius { get; private set;}

    public IReadOnlyDictionary<HexCoord, TileDefinitionId?> Tiles => _tiles;

    public IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> DeploymentPoints
        => _deploymentPoints.ToDictionary(kvp => kvp.Key, IReadOnlyList<HexCoord> (kvp) => kvp.Value);

    public IReadOnlyList<GlobalConditionDefinitionId> InitialGlobalConditions => _initialGlobalConditions;
    public IReadOnlyList<GlobalEvent> GlobalEvents => _globalEvents;

    // Constructor
    private MapDefinition(MapDefinitionId id, string name, int radius)
    {
        Id = id;
        Name = name;
        Radius = radius;
        
        InitBoard(radius);
        InitSpawnPoints();
    }
    public static MapDefinition Create(string name = "Default", int radius = 10)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Map name cannot be empty.", nameof(name));
        
        if (radius < 1)
            throw new DomainException("Map radius must be greater than 0.");

        return new MapDefinition(MapDefinitionId.New(), name, radius);
    }
    public static MapDefinition Load(MapDefinitionId id, string name = "Default", int radius = 10)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Map name cannot be empty.", nameof(name));
        
        if (radius < 1)
            throw new DomainException("Map radius must be greater than 0.");

        return new MapDefinition(id, name, radius);
    }

    // General
    public void ChangeName(string name) => Name = name;
    public void SetTile(TileDefinitionId tileDefinitionId, HexCoord coord)
    {
        if (!_tiles.ContainsKey(coord))
        {
            throw new DomainException($"Tile {coord} does not exist.");
        }
        
        _tiles[coord] = tileDefinitionId;
    }
    
    // Spawn points
    public void AddDeploymentPoint(PlayerSide side, HexCoord coord)
    {
        // Validation
        // Does side exist?
        if (!_deploymentPoints.TryGetValue(side, out var spawnPoints))
        {
            throw new DomainException($"Spawn point for side {side} does not exist.");
        }
        
        // Does spawn point already exist?
        if (!_tiles.ContainsKey(coord))
        {
            throw new DomainException($"Tile {coord} does not exist.");
        }

        spawnPoints.Add(coord);
    }
    public void RemoveDeploymentPoint(PlayerSide side, HexCoord coord)
    {
        if (!_deploymentPoints.TryGetValue(side, out var spawnPoints))
        {
            throw new DomainException($"Spawn point for side {side} does not exist.");
        }
        
        spawnPoints.Remove(coord);
    }
    public void ClearDeploymentPoints(PlayerSide side)
    {
        // Validation
        // Does side exist?
        if (!_deploymentPoints.TryGetValue(side, out var spawnPoints))
        {
            throw new DomainException($"Spawn point for side {side} does not exist.");
        }
        
        spawnPoints.Clear();
    }
    
    // Global conditions
    public void AddInitialGlobalCondition(GlobalConditionDefinitionId conditionDefinitionId)
    {
        _initialGlobalConditions.Add(conditionDefinitionId);
    }
    public void RemoveInitialGlobalCondition(GlobalConditionDefinitionId conditionDefinitionId)
    {
        _initialGlobalConditions.Remove(conditionDefinitionId);
    }
    public void ClearInitialGlobalConditions()
    {
        _initialGlobalConditions.Clear();
    }
    
    // Global events
    public void AddGlobalEvent(GlobalEvent globalEvent) => _globalEvents.Add(globalEvent);
    public void RemoveGlobalEvent(int turnNumber)
    {
        var eventToRemove = _globalEvents.FirstOrDefault(e => e.TurnNumber == turnNumber);
    }
    public void ClearGlobalEvents() => _globalEvents.Clear();
    
    private void InitBoard(int radius)
    {
        for (var q = -radius; q <= radius; q++)
        {
            var r1 = Math.Max(-radius, -q - radius);
            var r2 = Math.Min(radius, -q + radius);
            for (var r = r1; r <= r2; r++)
            {
                var coord = new HexCoord(q, r);
                _tiles[coord] = null;
            }
        }
    }
    private void InitSpawnPoints()
    {
        foreach (var side in Enum.GetValues<PlayerSide>())
            _deploymentPoints[side] = [];
    }
}