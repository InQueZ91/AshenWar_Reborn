using System.Collections.Generic;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Domain.Entities.Conditions;

public sealed class ConditionEffect
{
    private readonly List<IActionDefinition> _actions = [];

    public ITargetFilter? Filter { get; private set;}
    public IValidator? Guard { get; private set;}
    public IReadOnlyList<IActionDefinition> Actions => _actions;
    
    private ConditionEffect(){}
    public static ConditionEffect Create() => new();
    
    public void SetFilter(ITargetFilter? filter) => Filter = filter;
    public void SetGuard(IValidator? guard) => Guard = guard;
    
    public void AddAction(IActionDefinition action) => _actions.Add(action);
    public void RemoveAction(IActionDefinition action) => _actions.Remove(action);
    public void ClearActions() => _actions.Clear();
}