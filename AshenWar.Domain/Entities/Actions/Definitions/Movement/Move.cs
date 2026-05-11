using Domain.Interfaces.Actions;
using Domain.ValueObjects;

namespace Domain.Entities.Actions.Definitions.Movement;

public sealed class Move : IActionDefinition
{
    public HexCoord Destination { get; init; } = null!;
}