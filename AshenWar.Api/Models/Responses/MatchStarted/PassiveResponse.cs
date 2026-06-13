namespace AshenWar.Api.Models.Responses.MatchStarted;

public sealed record PassiveResponse
{
    public Guid PassiveId { get; init; }
    public Guid DefinitionId { get; init; }
    public required string Name { get; init; }
}