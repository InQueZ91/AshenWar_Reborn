using System.Numerics;
using Domain.Entities.Unit.Events;
using Tests.TestHelpers;

namespace Tests.Units;

public class UnitMoveTests
{
    [Fact]
    public void MoveTo_changes_coord_and_raises_event()
    {
        var unit = UnitBuilder.Default().Build();
        unit.ClearDomainEvents();

        var newCoord = new Vector2(2, 1);
        unit.MoveTo(newCoord);
        
        Assert.Equal(newCoord, unit.Coord);
        
        var events = unit.GetDomainEvents();
        Assert.Contains(events, e => e is UnitMoved);
    }
    
    [Fact]
    public void MoveTo_same_coord_does_not_raise_event()
    {
        var unit = UnitBuilder.Default().Build();
        unit.ClearDomainEvents();
        
        var sameCoord = unit.Coord;
        unit.MoveTo(sameCoord);
        
        Assert.Equal(sameCoord, unit.Coord);
        
        var events = unit.GetDomainEvents();
        Assert.Empty(events);
    }
}