using System.Text.Json;
using System.Text.Json.Serialization;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Entities.Actions.Definitions.Unit;

public sealed record SpawnUnit : IActionDefinition
{
    public UnitDefinitionId UnitDefinitionId { get; }
    public OwnerSource OwnerSource { get; } 
    public UserId? ExplicitOwner { get; }
    public PositionSource PositionSource { get; }
    public HexCoord? ExplicitPosition { get; }

    [JsonConstructor]
    private SpawnUnit(
        UnitDefinitionId unitDefinitionId,
        OwnerSource ownerSource,
        UserId? explicitOwner,
        PositionSource positionSource,
        HexCoord? explicitPosition)
    {
        if (ownerSource == OwnerSource.Explicit && explicitOwner == null)
            throw new JsonException("ExplicitOwner required");
        if (positionSource == PositionSource.Explicit && explicitPosition == null)
            throw new JsonException("ExplicitPosition required");
        
        UnitDefinitionId = unitDefinitionId;
        OwnerSource = ownerSource;
        ExplicitOwner = explicitOwner;
        PositionSource = positionSource;
        ExplicitPosition = explicitPosition;
    }
    
    // --- Factory methods ---
    
    // Caster owns the spawned unit, spawns at caster position
    public static SpawnUnit AtCaster(UnitDefinitionId unitDefinitionId)
        => new(unitDefinitionId, OwnerSource.Caster, null, PositionSource.Caster, null);
    
    // Caster owns, spawn at target position
    public static SpawnUnit AtTarget(UnitDefinitionId unitDefinitionId)
        => new(unitDefinitionId, OwnerSource.Caster, null, PositionSource.Target, null);
    
    // Explicit owner at explicit position - both required
    public static SpawnUnit Explicit(UnitDefinitionId unitDefinitionId, UserId owner, HexCoord position) 
        => new(unitDefinitionId, OwnerSource.Explicit, owner, PositionSource.Explicit, position);
    
    // Mix - explicit owner, target position
    public static SpawnUnit ExplicitOwnerAtTarget(UnitDefinitionId unitDefinitionId, UserId owner)
        => new(unitDefinitionId, OwnerSource.Explicit, owner, PositionSource.Target, null);
    
}