using Domain.Interfaces.Actions;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Tiles;

namespace Domain.Entities.Actions.Definitions.Tile;

public sealed class SpawnTile(TileDefinitionId tileDefinitionId, HexCoord Position) : IActionDefinition
{
    public TileDefinitionId TileDefinitionId { get; init; } = tileDefinitionId;
    public HexCoord Position { get; init; } = Position;
}