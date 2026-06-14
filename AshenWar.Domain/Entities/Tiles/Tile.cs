using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.Events.Tiles;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Entities.Tiles;

public sealed class Tile : DomainEntity, ITile
{
    private readonly List<TileCondition> _conditions = [];
    private readonly List<Passive> _passiveAbilities = [];
    
    public TileId Id { get; private set; }
    public TileDefinition Definition { get; }
    public TileDefinitionId DefinitionId => Definition.Id;
    public StatBlock Stats => Definition.BaseStats;
    public IReadOnlySet<EntityTag> Tags => Definition.Tags;
    
    // Runtime state
    public HexCoord? Position { get; private set;}
    public bool HasFog { get; private set;}
    public bool IsClaimed { get; private set;}
    public bool IsDestroyed { get; private set;}
    
    public IReadOnlyList<TileCondition> Conditions => _conditions.AsReadOnly();
    public IReadOnlyList<Passive> Passives => _passiveAbilities.AsReadOnly();
    
    // Constructor
    private Tile(TileId id,
        TileDefinition definition,
        HexCoord? position,
        bool hasFog,
        bool isClaimed,
        bool isDestroyed)
    {
        Id = id;
        Definition = definition;
        Position = position;
        HasFog = hasFog;
        IsClaimed = isClaimed;
        IsDestroyed = isDestroyed;
    }
    public static Tile Instantiate(TileDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
     
        return new Tile(TileId.New(), definition, null, false, false, false);
    }
    public static Tile Rehydrate(
        TileId id,
        TileDefinition definition,
        HexCoord? position,
        bool hasFog,
        bool isClaimed,
        bool isDestroyed,
        IEnumerable<TileCondition> conditions,
        IEnumerable<Passive> passives)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(definition);
        
        if (position is null && isClaimed)
            throw new DomainException("Tile must have a position if it is claimed.");
        
        if (position is not null && isDestroyed)
            throw new DomainException("Tile cannot be destroyed if it has a position.");
        
        var tile = new Tile(id, definition, position, hasFog, isClaimed, isDestroyed);
        
        // Bypass add/apply logics
        tile._conditions.AddRange(conditions);
        tile._passiveAbilities.AddRange(passives);

        return tile;
    }

    // Tile Management
    private void ThrowIfDestroyed()
    {
        if (IsDestroyed)
            throw new InvalidOperationException($"Tile at {Position} is destroyed.");
    }
    public void AddFog()
    {
        ThrowIfDestroyed();
        if (HasFog) return;
        HasFog = true;
     
        RaiseDomainEvent(new TileFogged(Id));
    }
    public void RemoveFog()
    {
        ThrowIfDestroyed();
        if (!HasFog) return;
        HasFog = false;
        
        RaiseDomainEvent(new TileRevealed(Id));
    }
    public void Destroy()
    {
        ThrowIfDestroyed();
        IsDestroyed = true;
        
        RaiseDomainEvent(new TileDestroyed(Id));
    }
    internal void Release()
    {
        ThrowIfDestroyed();
        IsClaimed = false;
        
        RaiseDomainEvent(new TileReleased(Id));
    }
    internal void Claim()
    {
        ThrowIfDestroyed();
        IsClaimed = true;
        
        RaiseDomainEvent(new TileClaimed(Id));
    }

    // IHasPassives
    public Passive? GetPassiveByDefinitionId(PassiveDefinitionId definitionId)
    {
        ArgumentNullException.ThrowIfNull(definitionId);
        return _passiveAbilities.FirstOrDefault(p => p.Definition.Id == definitionId);
    }

    // IPassive
    public void AddPassive(Passive ability)
    {
        _passiveAbilities.Add(ability);
    }
    public void RemovePassive(PassiveId passiveId)
    {
        _passiveAbilities.RemoveAll(p => p.Id == passiveId);
    }

    #region Conditions

    // IHasConditions
    public TileCondition? GetConditionById(ConditionId id)
    {
        return _conditions.FirstOrDefault(c => c.Id == id);
    }
    public TileCondition? GetConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.FirstOrDefault(c => c.Definition.ConditionTags.Overlaps(conditionTags));
    }
    public bool HasAnyConditionWithTag(params ConditionTag[] conditionTags)
    {
        return _conditions.Any(c => c.Definition.ConditionTags.Overlaps(conditionTags));
    }

    // ICondition
    public void ApplyCondition(TileCondition incoming)
    {
        var existing = _conditions.FirstOrDefault(s => s.Definition.Id == incoming.Definition.Id);
        if (existing is null) 
        {
            AddCondition(incoming);
            return;
        }
        
        incoming.Definition.StackingBehavior.Apply(existing, incoming, AddCondition, RemoveCondition);
    }
    public void RemoveCondition(ConditionId conditionId)
    {
        var effect = _conditions.FirstOrDefault(s => s.Id == conditionId);
        if (effect == null) return;
        
        RemoveCondition(effect);
    }
    
    private void AddCondition(ConditionBase condition)
    {
        if (condition is not TileCondition incoming)
            throw new ArgumentException("Condition must be of type UnitCondition.", nameof(condition));
        
        _conditions.Add(incoming);
        RaiseDomainEvent(new TileConditionApplied(Id, incoming.Definition.Id));
    }
    private void RemoveCondition(ConditionBase condition)
    {
        if (condition is not TileCondition incoming)
            throw new ArgumentException("Condition must be of type UnitCondition.", nameof(condition));
        
        _conditions.Remove(incoming);
        RaiseDomainEvent(new TileConditionRemoved(Id, incoming.Definition.Id));
    }

    #endregion
    
    // IHasStats
    public int GetFinalStat(StatDefinition stat)
    {
        return ModifierCalculator.Calculate(Stats.Get(stat), stat, _conditions, Tags);
    }
    
    internal void PlacedAt(HexCoord position) => Position = position;
    internal void Removed() => Position = null;
    
    internal void RehydrateCondition(TileCondition condition) => AddCondition(condition);
    internal void RehydratePassive(Passive passive) => AddPassive(passive);
}