using System;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Application.ValueObjects;

public abstract record ResolutionEvent();

public static class ResolutionEvents
{
    // Units
    public sealed record UnitDamaged(Guid UnitId, int Amount, int RemainingHealth) : ResolutionEvent;
    public sealed record UnitHealed(Guid UnitId, int Amount, int CurrentHealth) : ResolutionEvent;
    public sealed record UnitMoved(Guid UnitId, HexCoord From, HexCoord To) : ResolutionEvent;
    public sealed record UnitDied(Guid UnitId, HexCoord Position) : ResolutionEvent;
    public sealed record UnitSpawned(Guid UnitId, HexCoord Position) : ResolutionEvent;
    public sealed record UnitDespawned(Guid UnitId) : ResolutionEvent;
    public sealed record UnitConditionApplied(Guid UnitId, Guid ConditionDefinitionId) : ResolutionEvent;
    public sealed record UnitConditionRemoved(Guid UnitId, Guid ConditionDefinitionId) : ResolutionEvent;
    
    // Tiles
    public sealed record TileSpawned(Guid TileId, HexCoord Position) : ResolutionEvent;
    public sealed record TileDespawned(Guid TileId) : ResolutionEvent;
    public sealed record TileDestroyed(Guid TileId) : ResolutionEvent;
    public sealed record TileConditionApplied(Guid TileId, Guid ConditionDefinitionId) : ResolutionEvent;
    public sealed record TileConditionRemoved(Guid TileId, Guid ConditionDefinitionId) : ResolutionEvent;
    public sealed record TileFogged(Guid TileId) : ResolutionEvent;
    public sealed record TileRevealed(Guid TileId) : ResolutionEvent;

    // Global
    public sealed record GlobalConditionApplied(Guid ConditionDefinitionId) : ResolutionEvent;
    public sealed record GlobalConditionRemoved(Guid ConditionDefinitionId) : ResolutionEvent;

    // Match
    public sealed record MatchEnded(Guid MatchId, Guid WinnerId) : ResolutionEvent;
    public sealed record PlanningStarted(Guid MatchId, DateTimeOffset StartedAt, int DurationInSeconds) : ResolutionEvent;
}