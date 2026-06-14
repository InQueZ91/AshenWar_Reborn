namespace AshenWar.Api.Models.Responses.Definitions.Maps;

public sealed record MapTileResponse(HexCoordResponse Position, Guid TileDefinitionId);