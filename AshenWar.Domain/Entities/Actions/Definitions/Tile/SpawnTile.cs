using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;

namespace AshenWar.Domain.Entities.Actions.Definitions.Tile;

public sealed record SpawnTile(TileDefinitionId TileDefinitionId, HexCoord Position) : IActionDefinition;