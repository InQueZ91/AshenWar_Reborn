using System;
using System.Collections.Generic;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Units;

namespace AshenWar.Domain.Entities.Match.Definitions;

public sealed class MatchDefinition
{
    private readonly List<UnitDefinitionId> _blacklistedUnits = [];
    
    public MatchDefinitionId Id { get; private set; }
    public string Name { get; private set;} = "Default";
    
    public TimeSpan PlanningDurationSeconds { get; private set;}
    public int PowerLimit { get; private set;}
    public IReadOnlyList<UnitDefinitionId> BlacklistedUnits => _blacklistedUnits;

    // Constructor
    private MatchDefinition(MatchDefinitionId id, string name, TimeSpan planningDurationSeconds, int powerLimit)
    {
        Id = id;
        SetName(name);
        SetPlanningDuration(planningDurationSeconds);
        SetPowerLimit(powerLimit);
    }
    public static MatchDefinition Create(string name, TimeSpan planningDurationSeconds, int powerLimit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Match name cannot be empty.");
        
        if (planningDurationSeconds < TimeSpan.Zero)
            throw new DomainException("Planning duration must be positive.");
        
        if (powerLimit < 0)
            throw new DomainException("Power limit must be non-negative.");
        
        return new MatchDefinition(MatchDefinitionId.New(), name, planningDurationSeconds, powerLimit);
    }
    public static MatchDefinition Load(MatchDefinitionId id, string name, TimeSpan planningDurationSeconds, int powerLimit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Match name cannot be empty.");
        
        if (planningDurationSeconds < TimeSpan.Zero)
            throw new DomainException("Planning duration must be positive.");
        
        if (powerLimit < 0)
            throw new DomainException("Power limit must be non-negative.");
        
        return new MatchDefinition(id, name, planningDurationSeconds, powerLimit);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Match name cannot be empty.");
        
        Name = name;
    }
    public void SetPlanningDuration(TimeSpan planningDurationSeconds)
    {
        if (planningDurationSeconds < TimeSpan.Zero)
            throw new DomainException("Planning duration must be positive.");
        PlanningDurationSeconds = planningDurationSeconds;
    }
    public void SetPowerLimit(int powerLimit)
    {
        if (powerLimit < 0)
            throw new DomainException("Power limit must be non-negative.");
        PowerLimit = powerLimit;
    }

    // Blacklisted units
    public void AddBlacklistedUnit(UnitDefinitionId unitDefinitionId) => _blacklistedUnits.Add(unitDefinitionId);
    public void RemoveBlacklistedUnit(UnitDefinitionId unitDefinitionId) => _blacklistedUnits.Remove(unitDefinitionId);
    public void ClearBlacklistedUnits() => _blacklistedUnits.Clear();
}