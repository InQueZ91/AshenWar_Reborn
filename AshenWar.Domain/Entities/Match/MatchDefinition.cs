using System;
using System.Collections.Generic;
using Domain.Exceptions;
using Domain.ValueObjects.Identifiers.Match;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Entities.Match;

public sealed class MatchDefinition
{
    private readonly List<UnitDefinitionId> _blacklistedUnits = [];
    
    public MatchDefinitionId Id { get; private set; }
    public string Name { get; private set;} = "Default";
    
    public TimeSpan PlanningDuration { get; private set;}
    public int PowerLimit { get; private set;}
    public List<UnitDefinitionId> BlacklistedUnits => _blacklistedUnits;

    // Constructor
    private MatchDefinition(){} // EF Core
    private MatchDefinition(string name, TimeSpan planningDuration, int powerLimit)
    {
        Id = MatchDefinitionId.New();
        
        SetName(name);
        SetPlanningDuration(planningDuration);
        SetPowerLimit(powerLimit);
    }
    public static MatchDefinition Create(string name, TimeSpan planningDuration, int powerLimit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Match name cannot be empty.");
        
        if (planningDuration < TimeSpan.Zero)
            throw new DomainException("Planning duration must be positive.");
        
        if (powerLimit < 0)
            throw new DomainException("Power limit must be non-negative.");
        
        return new MatchDefinition(name, planningDuration, powerLimit);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Match name cannot be empty.");
        
        Name = name;
    }
    public void SetPlanningDuration(TimeSpan planningDuration)
    {
        if (planningDuration < TimeSpan.Zero)
            throw new DomainException("Planning duration must be positive.");
        PlanningDuration = planningDuration;
    }
    public void SetPowerLimit(int powerLimit)
    {
        if (powerLimit < 0)
            throw new DomainException("Power limit must be non-negative.");
        PowerLimit = powerLimit;
    }

    // Blacklisted units
    public void AddBlacklistedUnit(UnitDefinitionId unitDefinitionId) => BlacklistedUnits.Add(unitDefinitionId);
    public void RemoveBlacklistedUnit(UnitDefinitionId unitDefinitionId) => BlacklistedUnits.Remove(unitDefinitionId);
    public void ClearBlacklistedUnits() => BlacklistedUnits.Clear();
}