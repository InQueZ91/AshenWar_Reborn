using Domain.Entities.Unit;
using Domain.Entities.Unit.Events;
using Tests.TestHelpers;

namespace Tests.Units;

public class UnitStateTests
{
    [Fact]
    public void Ready_to_Exhausted_is_allowed_and_raises_event()
    {
        var unit = UnitBuilder.Default().Build(); // starts Ready
        unit.ClearDomainEvents();

        unit.Exhaust();

        Assert.Equal(UnitState.Exhausted, unit.State);
        Assert.Contains(unit.GetDomainEvents(), e => e is UnitExhausted);
    }

    [Fact]
    public void Exhausted_to_Ready_is_allowed()
    {
        var unit = UnitBuilder.Default().Build();
        unit.Exhaust();
        unit.ClearDomainEvents();
        
        unit.Ready();

        Assert.Equal(UnitState.Ready, unit.State);
        Assert.Contains(unit.GetDomainEvents(), e => e is UnitReady);
    }

    [Fact]
    public void Calling_same_state_is_no_op_and_raises_no_event()
    {
        var unit = UnitBuilder.Default().Build();
        unit.ClearDomainEvents();

        unit.Ready(); // already Ready

        Assert.Empty(unit.GetDomainEvents());
    }

    [Fact]
    public void Dead_unit_ignores_state_commands()
    {
        var unit = UnitBuilder.Default().Build();
        unit.TakeDamage(999);     // becomes dead
        unit.ClearDomainEvents();

        unit.Ready();
        unit.Exhaust();

        Assert.Equal(UnitState.Dead, unit.State);
        Assert.Empty(unit.GetDomainEvents());
    }
}