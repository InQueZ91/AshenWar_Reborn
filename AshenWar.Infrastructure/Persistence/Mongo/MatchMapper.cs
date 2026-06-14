using AshenWar.Application.Contracts.Definitions;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Entities.Tiles;
using AshenWar.Domain.Entities.Units;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Abilities;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;
using AshenWar.Domain.ValueObjects.Orders;
using AshenWar.Infrastructure.Definitions.Dtos.Maps;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Abilities;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Conditions;

namespace AshenWar.Infrastructure.Persistence.Mongo;

public sealed class MatchMapper(
    ITileDefinitionRepository tileDefinitionRepository,
    IUnitDefinitionRepository unitDefinitionRepository,
    IConditionDefinitionRepository conditionDefinitionRepository,
    IAbilityDefinitionRepository abilityDefinitionRepository,
    IPassiveDefinitionRepository passiveDefinitionRepository)
{
    public MatchDto ToDto(Match match)
    {
        return new MatchDto
        {
            Id = match.Id.Value,
            Phase = match.Phase,
            BlueSide = ToPlayerDto(match.BlueSide),
            RedSide = ToPlayerDto(match.RedSide),
            Board = ToBoardDto(match.Board),
            CurrentTurn = ToTurnDto(match.CurrentTurn),
            GlobalConditions = match.Conditions.Select(ToConditionDto).ToList(),
            PlanningDurationSeconds = (int)match.PlanningDurationSeconds.TotalSeconds,
            SpawnPoints = ToSpawnPointsDto(match.DeploymentPoints),
            GlobalEvents = match.GlobalEvents.Select(ToGlobalEventDto).ToList(),
        };
    }

    private static PlayerDto ToPlayerDto(Player player)
    {
        return new PlayerDto
        {
            UserId = player.UserId.Value,
            Side = player.Side,
            Rosters = player.Roster.Select(id => id.Value).ToList()
        };
    }

    private static BoardDto ToBoardDto(IBoard board)
    {
        return new BoardDto
        {
            Units = board.GetAllUnits().Select(ToUnitDto).ToList(),
            Tiles = board.GetAllTiles().Select(ToTileDto).ToList(),
        };
    }

    private static UnitDto ToUnitDto(IUnit unit)
    {
        return new UnitDto
        {
            UnitId = unit.Id.Value,
            OwnerId = unit.Owner.Value,
            DefinitionId = unit.DefinitionId.Value,
            Position = unit.Position,
            IsAlive = unit.IsAlive,
            CurrentHealth = unit.CurrentHealth,
            CurrentStamina = unit.CurrentStamina,
            CurrentSteps = unit.CurrentSteps,
            UnitConditions = unit.Conditions.Select(ToConditionDto).ToList(),
            Abilities = unit.Abilities.Select(ToAbilityDto).ToList(),
            Passives = unit.Passives.Select(ToPassiveDto).ToList(),
        };
    }

    private static TileDto ToTileDto(ITile tile)
    {
        return new TileDto
        {
            Id = tile.Id.Value,
            DefinitionId = tile.DefinitionId.Value,
            Position = tile.Position,
            HasFog = tile.HasFog,
            IsClaimed = tile.IsClaimed,
            IsDestroyed = tile.IsDestroyed,
            TileConditions = tile.Conditions.Select(ToConditionDto).ToList(),
            Passives = tile.Passives.Select(ToPassiveDto).ToList(),
        };
    }

    private static ConditionDto ToConditionDto(GlobalCondition condition)
    {
        return new ConditionDto
        {
            Id = condition.Id.Value,
            DefinitionId = condition.Definition.Id.Value,
            RemainingDuration = condition.RemainingDuration,
            RemainingStacks = condition.CurrentStacks,
        };   
    }
    private static ConditionDto ToConditionDto(UnitCondition condition)
    {
        return new ConditionDto
        {
            Id = condition.Id.Value,
            DefinitionId = condition.Definition.Id.Value,
            RemainingDuration = condition.RemainingDuration,
            RemainingStacks = condition.CurrentStacks,
        };
    }
    private static ConditionDto ToConditionDto(TileCondition condition)
    {
        return new ConditionDto
        {
            Id = condition.Id.Value,
            DefinitionId = condition.Definition.Id.Value,
            RemainingDuration = condition.RemainingDuration,
            RemainingStacks = condition.CurrentStacks,
        };
    }

    private static AbilityDto ToAbilityDto(Ability ability)
    {
        return new AbilityDto
        {
            Id = ability.Id.Value,
            DefinitionId = ability.Definition.Id.Value,
            RemainingCooldown = ability.RemainingCooldown,
        };
    }

    private static PassiveDto ToPassiveDto(Passive passive)
    {
        return new PassiveDto
        {
            Id = passive.Id.Value,
            DefinitionId = passive.Definition.Id.Value,
        };
    }

    private static TurnDto ToTurnDto(Turn turn)
    {
        return new TurnDto
        {
            TurnId = turn.Id.Value,
            TurnNumber = turn.TurnNumber,
            PlanningStartedAt = turn.PlanningStartedAt,
            PlanningDurationSeconds = (int)turn.PlanningDuration.TotalSeconds,
            BlueSubmitted = turn.BlueSubmitted,
            RedSubmitted = turn.RedSubmitted,
            IsResolved = turn.IsResolved,
            BlueOrders = ToPlayerOrdersDto(turn.BlueOrders),
            RedOrders = ToPlayerOrdersDto(turn.RedOrders),
        };
    }

    private static PlayerOrdersDto ToPlayerOrdersDto(PlayerOrders playerOrders)
    {
        return new PlayerOrdersDto
        {
            OwnerId = playerOrders.Owner.Value,
            Orders = playerOrders.Orders.Select(ToUnitOrderDto).ToList(),
        };
    }

    private static UnitOrderDto ToUnitOrderDto(UnitOrder unitOrder)
    {
        return new UnitOrderDto
        {
            UnitId = unitOrder.UnitId.Value,
            AbilityOrders = unitOrder.AbilityOrders.Select(ao => new AbilityOrderDto
            {
               AbilityId = ao.AbilityId.Value,
               Selections = ao.Selections.Select(ToStepSelectionDto).ToList()
            }).ToList()
        };
    }

    private static StepSelectionDto ToStepSelectionDto(StepSelection selection)
    {
        return selection.Target switch
        {
            Target.Unit u => new StepSelectionDto
                { AbilityStepId = selection.AbilityStepId.Value, TargetId = u.Id.Value, TargetType = TargetType.Unit },

            Target.Tile t => new StepSelectionDto
                { AbilityStepId = selection.AbilityStepId.Value, TargetId = t.Id.Value, TargetType = TargetType.Tile },

            Target.Self => new StepSelectionDto
                { AbilityStepId = selection.AbilityStepId.Value, TargetId = null, TargetType = TargetType.Self },
            
            _ => throw new InvalidOperationException($"Unknown Target type: {selection.Target.GetType().Name}")
        };
    }

    private static SpawnPointsDto ToSpawnPointsDto(IReadOnlyDictionary<PlayerSide, IReadOnlyList<HexCoord>> spawnPoints)
    {
        return new SpawnPointsDto
        {
            Blue = spawnPoints.TryGetValue(PlayerSide.Blue, out var blue) ? blue.ToList() : [],
            Red = spawnPoints.TryGetValue(PlayerSide.Red, out var red) ? red.ToList() : [],
        };
    }

    private static GlobalEventDto ToGlobalEventDto(GlobalEvent globalEvent)
    {
        return new GlobalEventDto(globalEvent.TurnNumber,
            globalEvent.Stacks,
            globalEvent.Pool.Select(id => id.Value).ToList()
        );
    }
    
    // --- Rehydration ---
    public async Task<Match> FromDto(MatchDto dto)
    {
        var blueSide = RehydratePlayer(dto.BlueSide);
        var redSide = RehydratePlayer(dto.RedSide);
        var currentTurn = RehydrateTurn(dto.CurrentTurn);
        var spawnPoints = new Dictionary<PlayerSide, IReadOnlyList<HexCoord>>{
            [PlayerSide.Blue] = dto.SpawnPoints.Blue,
            [PlayerSide.Red] = dto.SpawnPoints.Red,
        };
        var globalEvents = dto.GlobalEvents.Select(e => {
            return new GlobalEvent(
                e.TurnNumber,
                e.Stacks,
                e.Pool.Select(id => new GlobalConditionDefinitionId(id)).ToList());
        }).ToList();
        
        var units = await Task.WhenAll(dto.Board.Units.Select(RehydrateUnitAsync));
        var tiles = await Task.WhenAll(dto.Board.Tiles.Select(RehydrateTileAsync));
        var globalConditions = await RehydrateGlobalConditionsAsync(dto.GlobalConditions);
        
        return Match.Rehydrate(
            new MatchId(dto.Id),
            dto.Phase,
            blueSide,
            redSide,
            currentTurn,
            spawnPoints,
            globalEvents,
            TimeSpan.FromSeconds(dto.PlanningDurationSeconds),
            units,
            tiles,
            globalConditions
        );
    }
    
    private Player RehydratePlayer(PlayerDto dto)
    {
        return Player.Create(
            new UserId(dto.UserId),
            dto.Side,
            dto.Rosters.Select(id => new UnitDefinitionId(id)).ToList()
        );
    }

    private async Task<Unit> RehydrateUnitAsync(UnitDto dto)
    {
        var unitDefinition = await unitDefinitionRepository.FindAsync(new UnitDefinitionId(dto.DefinitionId));
        
        var abilities = await RehydrateAbilitiesAsync(dto.Abilities);
        var passives = await RehydratePassivesAsync(dto.Passives);
        var conditions = await RehydrateUnitConditionsAsync(dto.UnitConditions);

        return Unit.Rehydrate(
            new UnitId(dto.UnitId),
            new UserId(dto.OwnerId),
            unitDefinition,
            dto.Position,
            dto.CurrentHealth,
            dto.CurrentStamina,
            dto.CurrentSteps,
            conditions,
            abilities,
            passives);
    }

    private async Task<Tile> RehydrateTileAsync(TileDto dto)
    {
        var tileDefinition = await tileDefinitionRepository.FindAsync(new TileDefinitionId(dto.DefinitionId));
        
        var passives = await RehydratePassivesAsync(dto.Passives);
        var conditions = await RehydrateTileConditionAsync(dto.TileConditions);
        
        return Tile.Rehydrate(
            new TileId(dto.Id),
            tileDefinition,
            dto.Position,
            dto.HasFog,
            dto.IsClaimed,
            dto.IsDestroyed,
            conditions,
            passives);
    }

    private async Task<Ability[]> RehydrateAbilitiesAsync(List<AbilityDto> abilities)
    {
        return await Task.WhenAll(abilities.Select(async a =>
        {
            var abilityDefinition = await abilityDefinitionRepository.FindAbilityAsync(new AbilityDefinitionId(a.DefinitionId));
            if (abilityDefinition is null)
                throw new InvalidOperationException($"AbilityDefinition not found: {a.DefinitionId}");
            
            return Ability.Rehydrate(new AbilityId(a.Id), abilityDefinition, a.RemainingCooldown);
        }));
    }
    
    private async Task<Passive[]> RehydratePassivesAsync(List<PassiveDto> passives)
    {
        return await Task.WhenAll(passives.Select(async p =>
        {
            var passiveDefinition = await passiveDefinitionRepository.FindPassiveAsync(new PassiveDefinitionId(p.DefinitionId));
            
            return Passive.Rehydrate(new PassiveId(p.Id), passiveDefinition);
        }));
    }

    private async Task<UnitCondition[]> RehydrateUnitConditionsAsync(List<ConditionDto> conditions)
    {
        return await Task.WhenAll(conditions.Select(async c =>
        {
            var conditionDefinition = await conditionDefinitionRepository
                .GetUnitConditionAsync(new UnitConditionDefinitionId(c.DefinitionId));

            return UnitCondition.Rehydrate(
                new ConditionId(c.Id),
                conditionDefinition,
                c.RemainingDuration,
                c.RemainingStacks
            );
        }));
    }

    private async Task<TileCondition[]> RehydrateTileConditionAsync(List<ConditionDto> conditions)
    {
        return await Task.WhenAll(conditions.Select(async c =>
        {
            var conditionDefinition = await conditionDefinitionRepository
                .GetTileConditionAsync(new TileConditionDefinitionId(c.DefinitionId));

            return TileCondition.Rehydrate(
                new ConditionId(c.Id),
                conditionDefinition,
                c.RemainingDuration,
                c.RemainingStacks
            );
        }));
    }

    private async Task<GlobalCondition[]> RehydrateGlobalConditionsAsync(List<ConditionDto> conditions)
    {
        return await Task.WhenAll(conditions.Select(async c => {
            var definition = await conditionDefinitionRepository
                .GetGlobalConditionAsync(new GlobalConditionDefinitionId(c.DefinitionId));

            return GlobalCondition.Rehydrate(
                new ConditionId(c.Id),
                definition,
                c.RemainingDuration,
                c.RemainingStacks
            );
        }));
    }
        
    private Turn RehydrateTurn(TurnDto dto)
    {
        var blueOrders = RehydratePlayerOrders(dto.BlueOrders);
        var redOrders = RehydratePlayerOrders(dto.RedOrders);

        return Turn.Rehydrate(
            new TurnId(dto.TurnId),
            dto.TurnNumber,
            dto.PlanningStartedAt,
            TimeSpan.FromSeconds(dto.PlanningDurationSeconds),
            blueOrders,
            redOrders,
            dto.BlueSubmitted,
            dto.RedSubmitted,
            dto.IsResolved
        );
    }

    private PlayerOrders RehydratePlayerOrders(PlayerOrdersDto dto)
    {
        var orders = dto.Orders.Select(o => new UnitOrder
        {
            UnitId = new UnitId(o.UnitId),
            AbilityOrders = o.AbilityOrders.Select(ao => new AbilityOrder
            {
                AbilityId = new AbilityId(ao.AbilityId),
                Selections = ao.Selections.Select(RehydrateStepSelection).ToList()
            }).ToList()
        });
        
        return PlayerOrders.Create(new UserId(dto.OwnerId), orders);  
    }

    private StepSelection RehydrateStepSelection(StepSelectionDto dto)
    {
        Target target = dto.TargetType switch
        {
            TargetType.Unit => new Target.Unit(new UnitId(dto.TargetId!.Value)),
            TargetType.Tile => new Target.Tile(new TileId(dto.TargetId!.Value)),
            TargetType.Self => new Target.Self(),
            _ => throw new InvalidOperationException($"Unknown TargetType: {dto.TargetType}")
        };
        
        return new StepSelection(new AbilityStepId(dto.AbilityStepId), target);   
    }
}