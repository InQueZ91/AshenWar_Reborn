using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Tiles;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.ValueObjects.Orders;

public abstract record Target
{
    public abstract ITargetable Resolve(IBoard board, ITargetable caster);

    public sealed record Unit(UnitId Id) : Target
    {
        public override ITargetable Resolve(IBoard board, ITargetable caster)
            => board.GetUnitById(Id);
    }

    public sealed record Tile(TileId Id) : Target
    {
        public override ITargetable Resolve(IBoard board, ITargetable caster)
            => board.GetTileById(Id);
    }

    public sealed record Self : Target
    {
        public override ITargetable Resolve(IBoard board, ITargetable caster)
            => caster;
    }
}