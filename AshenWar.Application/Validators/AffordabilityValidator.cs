using System.Collections.Generic;
using System.Linq;
using Application.Events;
using Domain.Entities.Abilities.Active;
using Domain.Interfaces.Entities;

namespace Application.Validators;

public sealed class AffordabilityValidator
{
    public static List<AffordabilityFailure> Validate(IUnit source, ActiveAbility activeAbility)
    {
        var failures = new List<AffordabilityFailure>();
        
        // Readiness check - not a cost, separate concern
        if (!activeAbility.IsReady)
            failures.Add(new OnCooldown(activeAbility.RemainingCooldown));
        
        // Cost check - fully polymorphic, no knowledge of specific cost types
        failures.AddRange(from cost in activeAbility.Definition.Costs
            where !cost.CanAfford(source)
            select new CannotAffordCost(cost.GetFailureData(source)));

        return failures;
    }
}