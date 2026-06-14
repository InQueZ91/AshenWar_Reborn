using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Abilities.Passives.Triggers;
using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.Events.Match;
using AshenWar.Domain.Events.Tiles;
using AshenWar.Domain.Events.Units;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Entities.Match;

/// <summary>
/// Root aggregate for a match.
/// Owns Board, both Players, turn history and global ability.
/// All cross-entity coordination goes through Match - nothing reaches across aggregate boundaries directly. 
/// </summary>
public sealed class Match : DomainEntity, IMatch
{
    private readonly List<GlobalCondition> _conditions = [];
    
    public MatchId Id { get; private set; }
    public MatchPhase Phase { get; private set;}
    
    // Players
    public Player BlueSide { get; }
    public Player RedSide { get; }
    
    // Board
    public IReadOnlyBoard ReadOnlyBoardState => Board;
    public IBoard Board { get; private set; }
    public ITriggerRegistry TriggerRegistry { get; }

    // Turn
    public Turn CurrentTurn { get; private set; }
    
    // Global conditions
    public IReadOnlyList<GlobalCondition> Conditions => _conditions.AsReadOnly();
    
    // Might store only match definition id, then fetch from DB
    // Post-creation scalars from definitions
    public TimeSpan PlanningDurationSeconds { get; }
    public IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> DeploymentPoints { get; }
    public IReadOnlyList<GlobalEvent> GlobalEvents { get; }
    
    // Construction
    private Match(
        MatchId id,
        MatchPhase phase,
        Player blueSide,
        Player redSide,
        Board board,
        Turn currentTurn,
        IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> deploymentPoints,
        IReadOnlyList<GlobalEvent> globalEvents,
        TimeSpan planningDurationSeconds)
    {
        Id = id;
        Phase = phase;
        BlueSide = blueSide;
        RedSide = redSide;
        Board = board;
        TriggerRegistry = new TriggerRegistry();
        CurrentTurn = currentTurn;
        DeploymentPoints = deploymentPoints;
        GlobalEvents = globalEvents;
        PlanningDurationSeconds = planningDurationSeconds;
    }
    
    public static Match Instantiate(
        Player blueSide,
        Player redSide,
        IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> spawnPoints,
        IReadOnlyList<GlobalEvent> globalEvents,
        TimeSpan planningDurationSeconds)
    {
        ArgumentNullException.ThrowIfNull(blueSide);
        ArgumentNullException.ThrowIfNull(redSide);
        ArgumentNullException.ThrowIfNull(spawnPoints);
        ArgumentNullException.ThrowIfNull(globalEvents);

        // Turn 1 created immediately, begins on StartMatch
        var currentTurn = Turn.Instantiate(1, blueSide.UserId, redSide.UserId, planningDurationSeconds);
        
        return new Match(
            MatchId.New(),
            MatchPhase.Planning,
            blueSide,
            redSide,
            new Board(),
            currentTurn,
            spawnPoints,
            globalEvents,
            planningDurationSeconds);
    }
    
    public static Match Rehydrate(
        MatchId id,
        MatchPhase phase,
        Player blueSide,
        Player redSide,
        Turn currentTurn,
        IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> spawnPoints,
        IReadOnlyList<GlobalEvent> globalEvents,
        TimeSpan planningDurationSeconds,
        IEnumerable<Unit> units,
        IEnumerable<Tile> tiles,
        IEnumerable<GlobalCondition> globalConditions)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(blueSide);
        ArgumentNullException.ThrowIfNull(redSide);
        ArgumentNullException.ThrowIfNull(currentTurn);
        ArgumentNullException.ThrowIfNull(spawnPoints);
        ArgumentNullException.ThrowIfNull(globalEvents);
        
        var board = new Board().Rehydrate(units, tiles);

        var match = new Match(
            id,
            phase,
            blueSide,
            redSide,
            board,
            currentTurn,
            spawnPoints,
            globalEvents,
            planningDurationSeconds
        );

        // Global conditions - bypass stacking behavior
        match._conditions.AddRange(globalConditions);
        
        // Derived - rebuilt here because board is now fully populated
        foreach (var unit in board.GetAllUnits())
            foreach (var passive in unit.Passives)
                match.TriggerRegistry.Register(unit, passive);
        
        foreach (var tile in board.GetAllTiles())
            foreach (var passive in tile.Passives)
                match.TriggerRegistry.Register(tile, passive);
        
        return match;
    }
    
    // Unit Management
    public void SpawnUnit(Unit unit, HexCoord position)
    {
        ArgumentNullException.ThrowIfNull(unit);
        
        Board.PlaceUnit(unit, position);
        RaiseDomainEvent(new UnitSpawned(unit.Id, position));
    }
    public void DespawnUnit(IUnit unit)
    {
        ArgumentNullException.ThrowIfNull(unit);
        
        Board.RemoveUnit(unit);
        RaiseDomainEvent(new UnitDespawned(unit.Id));
    }

    // Tile Management
    public void SpawnTile(Tile tile, HexCoord position)
    {
        ArgumentNullException.ThrowIfNull(tile);
        
        Board.PlaceTile(tile, position);
        RaiseDomainEvent(new TileSpawned(tile.Id, position));
    }
    public void DespawnTile(ITile tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        
        Board.RemoveTile(tile);
        RaiseDomainEvent(new TileDespawned(tile.Id));
    }

    // Phase transition
    public void StartMatch()
    {
        if (Phase != MatchPhase.Planning)
            throw new InvalidOperationException("Match can only start from Planning phase.");

        CurrentTurn.BeginPlanning(PlanningDurationSeconds);
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
        
        RaiseDomainEvent(new ResolutionEnded(CurrentTurn.Id));
    }
    public Turn BeginNextTurn()
    {
        CurrentTurn = Turn.Instantiate(CurrentTurn.TurnNumber + 1, BlueSide.UserId, RedSide.UserId, PlanningDurationSeconds);
        return CurrentTurn;
    }
    public void DeclareWinner(UserId? winnerId)
    {
        Phase = MatchPhase.Ended;
        RaiseDomainEvent(new MatchEnded(Id, winnerId));
    }
    public MatchOutcome EvaluateOutcome()
    {
        var blueAlive = Board.GetUnitsForPlayer(BlueSide.UserId).Any(u => u.IsAlive);
        var redAlive = Board.GetUnitsForPlayer(RedSide.UserId).Any(u => u.IsAlive);
        
        return (blueAlive, redAlive) switch
        {
            (true, false) => MatchOutcome.Winner(BlueSide.UserId),
            (false, true) => MatchOutcome.Winner(RedSide.UserId),
            (false, false) => MatchOutcome.Draw(),
            (true, true) => MatchOutcome.Ongoing()
        };
    }
    
    #region Conditions

    // IHasConditions
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
    
    // ICondition
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