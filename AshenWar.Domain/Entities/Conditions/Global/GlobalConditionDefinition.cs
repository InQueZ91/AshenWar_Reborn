using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Entities;
using Domain.Interfaces.Match;
using Domain.ValueObjects.Identifiers.Conditions;

namespace Domain.Entities.Conditions.Global;

public sealed class GlobalConditionDefinition(string name) : ConditionDefinitionBase(name)
{
    public GlobalConditionDefinitionId Id { get; } = GlobalConditionDefinitionId.New();
    
    // Constructor
    public static GlobalConditionDefinition Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Condition name cannot be empty.", nameof(name));

        return new GlobalConditionDefinition(name);
    }

    public override IReadOnlyList<ITargetable> ResolveCandidates(ITargetable? holder, IBoard board)
    {
        return board.GetAllUnits().Concat<ITargetable>(board.GetAllTiles()).ToList();
    }
}