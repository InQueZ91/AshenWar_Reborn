using AshenWar.Domain.ValueObjects;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Abilities;
using AshenWar.Infrastructure.Persistence.Mongo.Dtos.Conditions;

namespace AshenWar.Infrastructure.Persistence.Mongo.Dtos;

public sealed record TileDto
{
    public Guid Id { get; init; }
    public Guid DefinitionId { get; init; }
    public HexCoord? Position { get; init; }
    public bool HasFog { get; init; }
    public bool IsClaimed { get; init; }
    public bool IsDestroyed { get; init; }
    public required List<ConditionDto> TileConditions { get; init; }
    public required List<PassiveDto> Passives { get; init; }
}