using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Domain.Entities.Actions.Definitions.Movement;

public sealed record Move(HexCoord Destination) : IActionDefinition;