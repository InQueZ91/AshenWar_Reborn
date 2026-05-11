using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities.Abilities.Passive.Triggers;
using Domain.Entities.Conditions;
using Domain.Entities.Conditions.Global;
using Domain.Entities.Tiles;
using Domain.Entities.Units;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Enums.Conditions;
using Domain.Events.Match;
using Domain.Events.Tiles;
using Domain.Events.Units;
using Domain.Interfaces;
using Domain.Interfaces.Match;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Tiles;
using Domain.ValueObjects.Identifiers.Units;
using Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace Domain.Entities.Match;

/// <summary>
/// Root aggregate for a match.
/// Owns Board, both Players, turn history and global ability.
/// All cross-entity coordination goes through Match - nothing reaches across aggregate boundaries directly. 
/// </summary>
public sealed class Match : MatchEntity, IMatchCommand
{
    private readonly List<Turn> _history = [];
    private readonly List<GlobalCondition> _conditions = [];
    private readonly Dictionary<UnitId, Unit> _units = [];
    private readonly Dictionary<TileId, Tile> _tiles = [];
    
    public MatchId Id { get; } = MatchId.New();
    public MatchPhase Phase { get; private set;}
    
    // Players
    public Player BlueSide { get; }
    public Player RedSide { get; }
    
    // Board
    public IBoard BoardState => Board;
    public IBoardCommand Board { get; } = new Board();
    public ITriggerRegistry TriggerRegistry { get; } = new TriggerRegistry();

    // Turn
    public Turn CurrentTurn { get; private set; }
    public IReadOnlyList<Turn> History => _history.AsReadOnly();
    
    // Phase
    public bool BlueDeployConfirmed { get; private set; }
    public bool RedDeployConfirmed { get; private set; }
    public bool BothDeployConfirmed => BlueDeployConfirmed && RedDeployConfirmed;
    
    // Global conditions
    public IReadOnlyList<GlobalCondition> Conditions => _conditions.AsReadOnly();
    
    // Post-creation scalars from definitions
    public TimeSpan PlanningDuration { get; }
    public IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> SpawnPoints { get; }
    public IReadOnlyList<GlobalEvent> GlobalEvents { get; }
    
    // Construction
    private Match(
        Player blueSide,
        Player redSide,
        IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> spawnPoints,
        IReadOnlyList<GlobalEvent> globalEvents,
        TimeSpan planningDuration)
    {
        BlueSide = blueSide;
        RedSide = redSide;
        SpawnPoints = spawnPoints;
        GlobalEvents = globalEvents;
        PlanningDuration = planningDuration;

        Phase = MatchPhase.Deploy;
        CurrentTurn = Turn.Begin(1, BlueSide.UserId, RedSide.UserId); // Turn 1 created immediately, begins on StartMatch
    }
    
    public static Match Create(
        Player blueSide,
        Player redSide,
        IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> spawnPoints,
        IReadOnlyList<GlobalEvent> globalEvents,
        TimeSpan planningDuration)
    {
        ArgumentNullException.ThrowIfNull(blueSide);
        ArgumentNullException.ThrowIfNull(redSide);
        ArgumentNullException.ThrowIfNull(spawnPoints);
        ArgumentNullException.ThrowIfNull(globalEvents);
        
        return new Match(blueSide, redSide, spawnPoints, globalEvents, planningDuration);
    }
    
    // Unit Management
    public Unit? GetUnitById(UnitId unitId)
    {
        return _units.GetValueOrDefault(unitId);
    }
    public List<Unit> GetUnitsForPlayer(UserId userId)
    {
        return _units.Values.Where(u => u.Owner == userId).ToList();
    }
    
    public void SpawnUnit(Unit unit, HexCoord position)
    {
        // Validation
        ArgumentNullException.ThrowIfNull(unit);
        ArgumentNullException.ThrowIfNull(position);
        
        Board.PlaceUnit(unit, position);
        _units.Add(unit.Id, unit);
        
        RaiseDomainEvent(new UnitSpawned(unit.Id, position));
    }
    public void DespawnUnit(Unit unit)
    {
        // Validation
        ArgumentNullException.ThrowIfNull(unit);
        
        Board.RemoveUnit(unit);
        _units.Remove(unit.Id);
        
        RaiseDomainEvent(new UnitDespawned(unit.Id));
    }

    // Tile Management
    public Tile? GetTileById(TileId tileId)
    {
        return _tiles.GetValueOrDefault(tileId);
    }
    public void SpawnTile(Tile tile, HexCoord position)
    {
        // Validation
        ArgumentNullException.ThrowIfNull(tile);
        ArgumentNullException.ThrowIfNull(position);
        
        Board.PlaceTile(tile, position);
        _tiles.Add(tile.Id, tile);
        
        RaiseDomainEvent(new TileSpawned(tile.Id, position));
    }
    public void DespawnTile(Tile tile)
    {
        // Validation
        ArgumentNullException.ThrowIfNull(tile);

        var unitOnTile = Board.RemoveTile(tile);
        if (unitOnTile is not null) 
            DespawnUnit(unitOnTile);
        
        _tiles.Remove(tile.Id);
        
        RaiseDomainEvent(new TileDespawned(tile.Id));
    }

    // Phase transition
    public void ConfirmDeploy(PlayerSide playerSide)
    {
        if (playerSide == PlayerSide.Blue)
            BlueDeployConfirmed = true;
        else
            RedDeployConfirmed = true;
    }
    public void StartMatch()
    {
        if (Phase != MatchPhase.Deploy)
            throw new InvalidOperationException("Match can only start from Deploy phase.");

        Phase = MatchPhase.Planning;
        CurrentTurn.BeginPlanning(PlanningDuration);
        
        RaiseDomainEvent(new MatchStarted(Id));
    }
    public void BeginResolution()
    {
        if (Phase != MatchPhase.Planning)
            throw new InvalidOperationException("Resolution can only begin from Planning phase.");

        Phase = MatchPhase.Resolution;
        
        RaiseDomainEvent(new ResolutionBegan(CurrentTurn.Id));
    }
    public void EndResolution()
    {
        if (Phase != MatchPhase.Resolution)
            throw new InvalidOperationException("EndResolution called outside of Resolution phase.");
        
        CurrentTurn.MarkAsResolved();
        _history.Add(CurrentTurn);
        
        RaiseDomainEvent(new ResolutionEnded(CurrentTurn.Id));
    }
    public Turn BeginNextTurn()
    {
        CurrentTurn = Turn.Begin(CurrentTurn.TurnNumber + 1, BlueSide.UserId, RedSide.UserId);
        return CurrentTurn;
    }
    public void DeclareWinner(UserId? winnerId)
    {
        Phase = MatchPhase.Ended;
        RaiseDomainEvent(new MatchEnded(Id, winnerId));
    }
    public MatchOutcome EvaluateOutcome()
    {
        var blueAlive = GetUnitsForPlayer(BlueSide.UserId).Any(u => u.IsAlive);
        var redAlive = GetUnitsForPlayer(RedSide.UserId).Any(u => u.IsAlive);
        
        return (blueAlive, redAlive) switch
        {
            (true, false) => MatchOutcome.Winner(BlueSide.UserId),
            (false, true) => MatchOutcome.Winner(RedSide.UserId),
            (false, false) => MatchOutcome.Draw(),
            (true, true) => MatchOutcome.Ongoing()
        };
    }
    
    #region Conditions

    // IConditionHolder
    public GlobalCondition? GetConditionById(ConditionId id)
    {
        return _conditions.FirstOrDefault(c => c.Id == id);
    }
    public GlobalCondition? GetConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.FirstOrDefault(c => c.Definition.ConditionTags.Overlaps(conditionTags));
    }
    public bool HasAnyConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.Any(c => c.Definition.ConditionTags.Overlaps(conditionTags));
    }
    
    // IConditionCommand
    public void ApplyCondition(GlobalCondition incoming)
    {
        var existing = _conditions.FirstOrDefault(gc => gc.Definition.Id == incoming.Definition.Id);
        if (existing is null) 
        {
            AddCondition(incoming);
            return;
        }
        
        incoming.Definition.StackingBehavior.Apply(existing, incoming, AddCondition, RemoveCondition);
    }
    public void RemoveCondition(ConditionId conditionId)
    {
        var effect = _conditions.FirstOrDefault(gc => gc.Id == conditionId);
        if (effect == null) return;
        
        RemoveCondition(effect);
    }
    
    // IConditionTarget
    private void AddCondition(ConditionBase condition)
    {
        if (condition is not GlobalCondition incoming)
            throw new ArgumentException("Condition must be of type GlobalCondition.", nameof(condition));
        
        _conditions.Add(incoming);
        RaiseDomainEvent(new GlobalConditionApplied(incoming.Definition.Id));
    }
    private void RemoveCondition(ConditionBase condition)
    {
        if (condition is not GlobalCondition incoming)
            throw new ArgumentException("Condition must be of type GlobalCondition.", nameof(condition));
        
        _conditions.Remove(incoming);
        RaiseDomainEvent(new GlobalConditionRemoved(incoming.Definition.Id));
    }

    #endregion

}