using AshenWar.Domain.Enums;
using AshenWar.Infrastructure.Definitions.Dtos.Maps;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Conditions;

namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record MatchDto
{
    public Guid Id { get; init; }
    public MatchPhase Phase { get; init; }
    public required PlayerDto BlueSide { get; init; }
    public required PlayerDto RedSide { get; init; }
    public required BoardDto Board { get; init; }
    public required TurnDto CurrentTurn { get; init; }
    public required List<ConditionDto> GlobalConditions { get; init; }
    public int PlanningDurationSeconds { get; init; }
    public required SpawnPointsDto SpawnPoints { get; init; }
    public required List<GlobalEventDto> GlobalEvents { get; init; }
}