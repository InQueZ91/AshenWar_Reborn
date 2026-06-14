using AshenWar.Infrastructure.Definitions.Dtos.Costs;

namespace AshenWar.Infrastructure.Definitions.Dtos.Abilities;

public sealed record AbilityDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public required AbilityStatsDto Stats { get; init; }
    public IEnumerable<CostDefinitionDto> Costs { get; init; } = [];
    public required IEnumerable<AbilityStepDto> Steps { get; init; }
    public IEnumerable<string> Tags { get; init; } = [];
}