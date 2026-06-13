using System.Collections.Generic;
using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Conditions;

namespace AshenWar.Domain.Interfaces.Conditions;

/// <summary>
/// Public contract - what the outside world sees on an entity that holds conditions
/// Used by ConditionExecutor, ModifierCalculator, and effect validators.
/// </summary>
public interface IHasConditions<TCondition> where TCondition : ConditionBase
{
    IReadOnlyList<TCondition> Conditions { get; }
    bool HasAnyConditionWithTag(params ConditionTag[] conditionTags);
    
    // Find existing instance of specific condition definition
    // Used by stacking handler to decide merge behavior
    TCondition? GetConditionById(ConditionId id);
    TCondition? GetConditionWithTag(params ConditionTag[] conditionTags);
}