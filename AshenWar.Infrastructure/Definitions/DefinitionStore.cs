using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Entities.Effects;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Match.Definitions;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Infrastructure.Definitions;

/// <summary>
/// Singleton in-memory cache of all game definitions.
/// Populated once at startup by DefinitionLoader.
/// Handlers never touch files - they go through InMemory repositories that wrap this store
/// </summary>
public sealed class DefinitionStore
{
    public Dictionary<UnitDefinitionId, UnitDefinition> Units { get; } = [];
    public Dictionary<AbilityDefinitionId, AbilityDefinition> Abilities { get; } = [];
    public Dictionary<PassiveDefinitionId, PassiveDefinition> Passives { get; } = [];
    public Dictionary<EffectDefinitionId, EffectDefinition> Effects { get; } = [];
    public Dictionary<MapDefinitionId, MapDefinition> Maps { get; } = [];
    public Dictionary<MatchDefinitionId, MatchDefinition> MatchDefinitions { get; } = [];
    public Dictionary<TileDefinitionId, TileDefinition> Tiles { get; } = [];
    public Dictionary<GlobalConditionDefinitionId, GlobalConditionDefinition> GlobalConditions { get; } = [];
    public Dictionary<TileConditionDefinitionId, TileConditionDefinition> TileConditions { get; } = [];
    public Dictionary<UnitConditionDefinitionId, UnitConditionDefinition> UnitConditions { get; } = [];
}