using AshenWar.Domain.ValueObjects;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Abilities;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Conditions;

namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record UnitDto()
{
    public Guid UnitId { get; init; }
    public Guid OwnerId { get; init; }
    public Guid DefinitionId { get; init; }
    public HexCoord? Position { get; init; }
    public bool IsAlive { get; init; }
    public int CurrentHealth { get; init; }
    public int CurrentStamina { get; init; }
    public int CurrentSteps { get; init; }
    public required List<ConditionDto> UnitConditions { get; init; }
    public required List<AbilityDto> Abilities { get; init; }
    public required List<PassiveDto> Passives { get; init; }
}