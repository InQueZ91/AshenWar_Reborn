using Domain.Interfaces.Entities;
using Domain.ValueObjects;

namespace Domain.Entities.Match;

/// <summary>
/// Sentinel source for actions initiated by the world-global events,
/// match-level effects with no caster. Position is always null.
/// Handlers must not assume a source has a position or owner.
/// </summary>
public sealed class WorldSource : ITargetable
{
    public static readonly WorldSource Instance = new();
    public HexCoord? Position => null;
}