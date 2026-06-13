namespace AshenWar.Api.Models.Responses.Match;

public sealed record MatchUnitResponse
{
    public Guid UnitId { get; init; }
    public Guid DefinitionId { get; init; }
    public Guid OwnerId { get; init; }
    public string Name { get; init; } = "";
    public bool IsAlive { get; init; }
    public int CurrentHealth { get; init; }
    public int CurrentStamina { get; init; }
    public int CurrentSteps { get; init; }
    public HexCoordResponse? Position { get; init; }
    public IReadOnlyList<MatchConditionResponse> Conditions { get; init; } = [];
}