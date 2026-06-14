namespace AshenWar.Api.Models.Responses.MatchStarted;

public sealed record MatchStartedUnitResponse
{
    public Guid UnitId { get; init; }
    public Guid OwnerId { get; init; }
    public Guid DefinitionId { get; init; }
    public HexCoordResponse? Position { get; init; }
    public required UnitStatsResponse FinalStats { get; init; }
    public required UnitResourcesResponse Resources { get; init; }
    public IReadOnlyList<AbilityResponse> Abilities { get; init; } = [];
    public IReadOnlyList<PassiveResponse> Passives { get; init; } = [];
    public IReadOnlyList<ConditionResponse> Conditions { get; init; } = [];   
}