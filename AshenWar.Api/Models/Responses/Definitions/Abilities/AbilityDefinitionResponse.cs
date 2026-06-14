namespace AshenWar.Api.Models.Responses.Definitions.Abilities;

public sealed record AbilityDefinitionResponse
{
    public string Name { get; init; } = "";
    public required AbilityStatsResponse BaseStats { get; init; }
    public IReadOnlyList<AbilityCostResponse> Costs { get; init; } = [];
    public IReadOnlyList<AbilityStepResponse> Steps { get; init; } = [];
    public IReadOnlyList<string> Tags { get; init; } = [];
}