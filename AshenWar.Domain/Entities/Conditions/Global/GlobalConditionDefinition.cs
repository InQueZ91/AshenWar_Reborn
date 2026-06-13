using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.Interfaces.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Conditions;

namespace AshenWar.Domain.Entities.Conditions.Global;

public sealed class GlobalConditionDefinition : ConditionDefinitionBase
{
    public GlobalConditionDefinitionId Id { get; }
    
    // Constructor
    private GlobalConditionDefinition(GlobalConditionDefinitionId id, string name) : base(name) => Id = id;
    public static GlobalConditionDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));

        return new GlobalConditionDefinition(GlobalConditionDefinitionId.New(), name);
    }
    public static GlobalConditionDefinition Load(GlobalConditionDefinitionId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));

        return new GlobalConditionDefinition(id, name);
    }

    public override IReadOnlyList<ITargetable> ResolveCandidates(ITargetable? holder, IReadOnlyBoard readOnlyBoard)
    {
        return holder is IReadOnlyUnit or IReadOnlyTile ? [] : readOnlyBoard.GetAll().ToList();
    }
}