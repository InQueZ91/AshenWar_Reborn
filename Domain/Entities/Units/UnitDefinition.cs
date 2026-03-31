using System;
using System.Collections.Generic;
using Domain.Entities.Abilities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Units;

public sealed class UnitDefinition : Entity
{
    // Identifiers
    public UnitDefinitionId Id { get; init; }
    public string Name { get; private set; }
    
    // References
    public VisualId VisualId { get; private set; }
    
    // Properties
    public UnitStats BaseStats { get; private set; }
    public List<AbilityDefinition> Abilities { get; private set; } = new List<AbilityDefinition>();

    // Constructor
    private UnitDefinition(VisualId visualId, string name, UnitStats baseStats)
    {
        Id = UnitDefinitionId.New();
        VisualId = visualId;
        Name = name;
        BaseStats = baseStats;
    }
    public static UnitDefinition Create(VisualId visualId, string name, UnitStats baseStats)
    {
        return new UnitDefinition(visualId, name, baseStats);
    }
    
    // Methods
    public void ChangeName(string newName) => Name = newName;
    public void ChangeVisualId(VisualId visualId) => VisualId = visualId;
    public void SetBaseStats(UnitStats newBaseStats) => BaseStats = newBaseStats;

    public void AddAbility(AbilityDefinition abilityDefinition) => Abilities.Add(abilityDefinition);
    public void RemoveAbility(AbilityDefinitionId abilityDefinitionId) => Abilities.RemoveAll(a => a.Id == abilityDefinitionId);
}