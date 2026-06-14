using AshenWar.Api.Models.Responses.Definitions.Abilities;
using AshenWar.Api.Models.Responses.Definitions.Passives;

namespace AshenWar.Api.Models.Responses.Definitions.Units;

public sealed record UnitSummaryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public Guid VisualId { get; init; }
    public required UnitStatsResponse BaseStats { get; init; }
    public IReadOnlyList<AbilitySummaryResponse> Abilities { get; init; } = [];
    public IReadOnlyList<PassiveSummaryResponse> Passives { get; init; } = [];
    public IReadOnlyList<string> Tags { get; init; } = [];
};