using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.Validators.Primitives;

public sealed record SourceHasStatValidator(int Minimum, StatDefinition Stat) : IValidator
{
    public bool Check(ActionContext context)
    { 
        if (context.Source is not IUnit unit) return false;
        
        var baseValue = unit.Stats.Get(Stat);
        var effective = ModifierCalculator.Calculate(baseValue, Stat, unit.Conditions, unit.Tags);
        return effective >= Minimum;
    }
}