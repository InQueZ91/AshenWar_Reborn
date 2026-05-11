using System.Collections.Generic;
using System.Linq;
using Application.Enums;
using Application.Exceptions;
using Domain.Entities.Match;
using Domain.Entities.Stats;
using Domain.Entities.Units;

namespace Application.Validators;

public sealed class RosterValidator
{
    public static void Validate(IReadOnlyList<UnitDefinition> roster, MatchDefinition matchDefinition)
    {
        var failures = new List<RosterFailure>();
        
        var sumOfUnitPower = roster.Select(u => u.BaseStats.Get(StatDefinition.Power)).Sum();
        if (sumOfUnitPower > matchDefinition.PowerLimit) 
            failures.Add(RosterFailure.PowerLimitExceeded);

        var blacklistSet = matchDefinition.BlacklistedUnits.ToHashSet();
        if (roster.Any(r => blacklistSet.Contains(r.Id)))
            failures.Add(RosterFailure.UnitOnBlacklist);
        
        if (failures.Count > 0)
            throw new RosterValidationException(failures);
    }
}