using System.Text.Json;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Abilities.Passives.Triggers;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Entities.Effects;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Match.Definitions;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Enums;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using AshenWar.Infrastructure.Definitions.Converters;
using AshenWar.Infrastructure.Definitions.Dtos;
using AshenWar.Infrastructure.Definitions.Dtos.Abilities;
using AshenWar.Infrastructure.Definitions.Dtos.Conditions;
using AshenWar.Infrastructure.Definitions.Dtos.Maps;
using Microsoft.Extensions.Logging;

namespace AshenWar.Infrastructure.Definitions;

/// <summary>
/// Reads JSON definition files from the disk at startup and populates DefinitionStore.
/// Call LoadAll() once in Program.cs before app.Run().
/// JSON files live in /definitions/ alongside the binary.
/// </summary>
public sealed partial class DefinitionLoader(DefinitionStore store, ILogger<DefinitionLoader> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new IdConverterFactory(),
            new ValueSourceConverter(),
        }
    };

    public void LoadAll(string rootPath)
    {
        LogLoadingStarted(rootPath);
        
        LoadEffectDefinitions(Path.Combine(rootPath, "effects"));
        LoadConditionDefinitions(Path.Combine(rootPath, "conditions"));
        LoadPassiveDefinitions(Path.Combine(rootPath, "passives"));
        LoadAbilityDefinitions(Path.Combine(rootPath, "abilities"));
        LoadTileDefinitions(Path.Combine(rootPath, "tiles"));
        LoadUnitDefinitions(Path.Combine(rootPath, "units"));
        LoadMapDefinitions(Path.Combine(rootPath, "maps"));
        LoadMatchDefinitions(Path.Combine(rootPath, "matches"));

        LogDefinitionsLoaded(
            store.MatchDefinitions.Count,
            store.Maps.Count,
            store.Units.Count,
            store.Tiles.Count,
            store.Abilities.Count,
            store.Passives.Count,
            store.GlobalConditions.Count,
            store.TileConditions.Count,
            store.UnitConditions.Count,
            store.Effects.Count
        );
    }
    
    private void LoadEffectDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<EffectDto>(file);
                var id = new EffectDefinitionId(dto.Id);
                var definition = EffectDefinition.Load(id, dto.Name);
                
                // Load actions
                foreach (var action in dto.Actions)
                    definition.AddAction(action);

                store.Effects[id] = definition;
            }
            catch (Exception ex)
            {
                LogLoadFailed("effect", file, ex);
            }
        }
    }
    
    private void LoadConditionDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<ConditionDefinitionBaseDto>(file);
                var definition = dto.ToDomain();

                switch (definition)
                {
                    case UnitConditionDefinition u:
                        store.UnitConditions[u.Id] = u; break;
                    case TileConditionDefinition t:
                        store.TileConditions[t.Id] = t; break;
                    case GlobalConditionDefinition g:
                        store.GlobalConditions[g.Id] = g; break;
                }
            }
            catch (Exception ex)
            {
                LogLoadFailed("condition", file, ex);
            }
        }
    }

    private void LoadPassiveDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<PassiveDefinitionDto>(file);
                var id = new PassiveDefinitionId(dto.Id);
                var definition = PassiveDefinition.Load(id, dto.Name);
                
                // Load passive triggers
                foreach (var triggerDto in dto.PassiveTriggers)
                {
                    var passiveTrigger = PassiveTrigger.Create();
                    passiveTrigger.SetShape(triggerDto.Shape);
                    passiveTrigger.SetFilter(triggerDto.Filter);
                    passiveTrigger.SetGuard(triggerDto.Guard);

                    foreach (var tag in triggerDto.Tags)
                        passiveTrigger.AddTag(EntityTag.FromName(tag));
                    
                    foreach (var entry in triggerDto.Triggers)
                        passiveTrigger.AddTrigger(entry);
                    
                    foreach (var actions in triggerDto.Actions)
                        passiveTrigger.AddAction(actions);
                    
                    foreach (var effectId in triggerDto.Effects)
                        passiveTrigger.AddEffect(new EffectDefinitionId(effectId));
                    
                    definition.AddPassiveTrigger(passiveTrigger);
                }

                store.Passives[id] = definition;
            }
            catch (Exception ex)
            {
                LogLoadFailed("passive ability", file, ex);
            }
        }
    }
    
    private void LoadAbilityDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<AbilityDefinitionDto>(file);
                var id = new AbilityDefinitionId(dto.Id);
                var stats = LoadAbilityStats(dto.Stats);
                var definition = AbilityDefinition.Load(id, dto.Name, stats);

                // Load costs
                foreach (var costDto in dto.Costs)
                    definition.AddCost(costDto.ToDomain());
                
                // Load steps
                foreach (var stepDto in dto.Steps)
                {
                    var step = AbilityStep.Create(stepDto.Shape);
                    
                    if (stepDto.Validator != null)
                        step.SetValidator(stepDto.Validator);
                    
                    if (stepDto.Filter != null)
                        step.SetFilter(stepDto.Filter);
                    
                    foreach (var actions in stepDto.Actions)
                        step.AddAction(actions);

                    foreach (var effectId in stepDto.Effects)
                        step.AddEffect(new EffectDefinitionId(effectId));

                    definition.AddStep(step);
                }
                
                // Load tags
                foreach (var tag in dto.Tags)
                    definition.AddTag(EntityTag.FromName(tag));

                store.Abilities[id] = definition;
            }
            catch (Exception ex)
            {
                LogLoadFailed("active ability", file, ex);
            }
        }
    }

    private void LoadTileDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<TileDefinitionDto>(file);
                var id = new TileDefinitionId(dto.Id);
                var visualId = new VisualId(dto.VisualId);
                var stats = LoadTileStats(dto.Stats);
                var definition = TileDefinition.Load(id, visualId, dto.Name, stats);

                // Load passive abilities
                foreach (var abilityId in dto.Passives)
                    definition.AddPassive(new PassiveDefinitionId(abilityId));

                // Load tags
                foreach (var tag in dto.Tags)
                    definition.AddTag(EntityTag.FromName(tag));

                store.Tiles[id] = definition;
            }
            catch (Exception ex)
            {
                LogLoadFailed("tile", file, ex);
            }
        }
    }
    
    private void LoadUnitDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<UnitDefinitionDto>(file);
                var id = new UnitDefinitionId(dto.Id);
                var visualId = new VisualId(dto.VisualId);
                var stats = LoadUnitStats(dto.Stats);
                var definition = UnitDefinition.Load(id, visualId, dto.Name, stats);

                // Load active abilities
                foreach (var abilityId in dto.Abilities)
                    definition.AddAbility(new AbilityDefinitionId(abilityId));

                // Load passive abilities
                foreach (var abilityId in dto.Passives)
                    definition.AddPassive(new PassiveDefinitionId(abilityId));
                
                // Load tags
                foreach (var tag in dto.Tags)
                    definition.AddTag(EntityTag.FromName(tag));

                store.Units[id] = definition;
            }
            catch (Exception ex)
            {
                LogLoadFailed("unit", file, ex);
            }
        }
    }

    private void LoadMapDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<MapDefinitionDto>(file);
                var id = new MapDefinitionId(dto.Id);
                var definition = MapDefinition.Load(id, dto.Name, dto.Radius);

                // Load tiles
                foreach (var tileEntry in dto.Tiles)
                    definition.SetTile(new TileDefinitionId(tileEntry.TileDefinitionId), tileEntry.Coord);

                // Load spawn points
                foreach (var (sideKey, coords) in dto.DeploymentPoints ?? [])
                {
                    var side = Enum.Parse<PlayerSide>(sideKey, ignoreCase: true);
                    foreach (var coordDto in coords)
                        definition.AddDeploymentPoint(side, new HexCoord(coordDto.Q, coordDto.R));
                }
                
                // Load global events
                foreach (var eventDto in dto.GlobalEvents)
                {
                    var pool = eventDto.Pool.Select(g => new GlobalConditionDefinitionId(g)).ToList();
                    definition.AddGlobalEvent(new GlobalEvent(eventDto.TurnNumber, eventDto.Stacks, pool));
                }
                
                // Load initial global conditions
                foreach (var conditionDto in dto.InitialGlobalConditions)
                {
                    var condition = new GlobalConditionDefinitionId(conditionDto);
                    definition.AddInitialGlobalCondition(condition);
                }
                
                store.Maps[id] = definition;
            }
            catch (Exception ex)
            {
                LogLoadFailed("map", file, ex);
            }
        }
    }
    
    private void LoadMatchDefinitions(string folder)
    {
        foreach (var file in SafeEnumerateFiles(folder))
        {
            try
            {
                var dto = Deserialize<MatchDefinitionDto>(file);
                var id = new MatchDefinitionId(dto.Id);
                var planningDuration = TimeSpan.FromSeconds(dto.PlanningDurationSeconds);
                var definition = MatchDefinition.Load(id, dto.Name, planningDuration, dto.PowerLimit);
                
                // Load blacklisted units
                foreach (var blackList in dto.BlacklistedUnits)
                    definition.AddBlacklistedUnit(new UnitDefinitionId(blackList));

                store.MatchDefinitions[id] = definition;
            }
            catch (Exception ex)
            {
                LogLoadFailed("match", file, ex);
            }
        }
    }

    // Helper
    private static TileStats LoadTileStats(TileStatsDto dto)
    {
        var block = new Dictionary<StatDefinition, int>
        {
            [StatDefinition.MovementCost] = dto.MovementCost
        };
        
        return new TileStats(block);   
    }
    
    private static AbilityStats LoadAbilityStats(AbilityStatsDto dto)
    {
        var block = new Dictionary<StatDefinition, int>
        {
            [StatDefinition.Damage] = dto.Damage,
            [StatDefinition.Cooldown] = dto.Cooldown,
            [StatDefinition.Range] = dto.Range,
        };
        
        return new AbilityStats(block);
    }
    
    private static UnitStats LoadUnitStats(UnitStatsDto dto)
    {
        var block = new Dictionary<StatDefinition, int>
        {
            [StatDefinition.Power] = dto.Power,
            [StatDefinition.Health] = dto.Health,
            [StatDefinition.Stamina] = dto.Stamina,
            [StatDefinition.Steps] = dto.Steps,
            [StatDefinition.Speed] = dto.Speed,
            [StatDefinition.Vision] = dto.Vision
        };
        
        return new UnitStats(block);
    }
    
    private static T Deserialize<T>(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name} from {path}");
    }
    
    private static IEnumerable<string> SafeEnumerateFiles(string folder)
    {
        return !Directory.Exists(folder) ? [] : Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);
    }
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Loading definitions from {Path}")]
    private partial void LogLoadingStarted(string path);

    [LoggerMessage(Level = LogLevel.Information,
        Message =
            "Definition loaded - Matches: {MD}, Maps: {M}, Units: {U}, Tiles: {T}, Abilities: {A}, Passives: {P}, GlobalConditions: {GC}, TileConditions {TC}, UnitConditions {UC}, Effects: {E} ")]
    private partial void LogDefinitionsLoaded(int md, int m, int u, int t, int a, int p, int gc, int tc, int uc, int e);
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to load {Type} from {File}")]
    private partial void LogLoadFailed(string type, string file, Exception ex);
}