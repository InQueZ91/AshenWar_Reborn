using System.Numerics;
using Domain.Entities.Unit;
using Domain.ValueObjects;

namespace Tests.TestHelpers;

public sealed class UnitBuilder
{
    private PlayerId _owner = PlayerId.New();
    private Vector2 _coord = new(0, 0);
    private UnitStats _baseStats = new()
    {
        Power = 5,
        Health = 10,
        Stamina = 6,
        Steps = 4,
        Speed = 3,
        Vision = 2,
    };
    
    public static UnitBuilder Default() => new();
    public UnitBuilder WithOwner(PlayerId owner)
    {
        _owner = owner;
        return this;
    }
    public UnitBuilder WithCoord(Vector2 coord)
    {
        _coord = coord;
        return this;
    }
    public UnitBuilder WithStats(UnitStats stats)
    {
        _baseStats = stats;
        return this;
    }
    
    public Unit Build() => Unit.Create(_owner, _coord, "test", _baseStats);
}