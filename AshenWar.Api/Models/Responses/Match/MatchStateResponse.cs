namespace AshenWar.Api.Models.Responses.Match;

public sealed record MatchStateResponse(IReadOnlyList<MatchUnitResponse> Units);