using Domain.Entities.Unit.Events;
using Tests.TestHelpers;

namespace Tests.Units;

public class UnitDamageTests
{
    [Fact]
    public void TakeDamage_more_than_health_sets_health_to_zero()
    {
        var unit = UnitBuilder.Default().Build();
        unit.ClearDomainEvents();
        
        unit.TakeDamage(999);
        
        Assert.Equal(0, unit.Health); 
    }
    
    [Fact]
    public void TakeDamage_to_zero_raises_UnitDied_once()
    {
        var unit = UnitBuilder.Default().Build();
        unit.ClearDomainEvents();
        
        unit.TakeDamage(999);
        unit.TakeDamage(999);
        
        var events = unit.GetDomainEvents();
        
        var deathEvents = events.Where(e => e is UnitDied).ToList();
     
        Assert.Single(deathEvents);
    }
}