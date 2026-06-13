using System.Collections.Generic;
using System.Linq;
using AshenWar.Application.Events;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Validators;

public sealed class AffordabilityValidator
{
    public static List<AffordabilityFailure> Validate(IReadOnlyUnit source, Ability ability)
    {
        var failures = new List<AffordabilityFailure>();
        
        // Readiness check - not a cost, separate concern
        if (!ability.IsReady)
            failures.Add(new OnCooldown(ability.RemainingCooldown));
        
        // Cost check - fully polymorphic, no knowledge of specific cost types
        failures.AddRange(from cost in ability.Definition.Costs
            where !cost.CanAfford(source)
            select new CannotAffordCost(cost.GetFailureData(source)));

        return failures;
    }
}