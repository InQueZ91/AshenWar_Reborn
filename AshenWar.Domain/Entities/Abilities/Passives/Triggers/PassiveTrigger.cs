using System;
using System.Collections.Generic;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Entities.Abilities.Passives.Triggers;

public sealed class PassiveTrigger
{
    private readonly HashSet<EntityTag> _tags = [];
    private readonly List<TriggerEntry> _triggers = []; // treat like OR logic set of triggers
    private readonly List<IActionDefinition> _actions = [];
    private readonly List<EffectDefinitionId> _effects = [];
    
    public PassiveTriggerId Id { get; } = PassiveTriggerId.New();
    public ITargetShape? Shape { get; private set; }
    public ITargetFilter? Filter { get; private set; }
    public IValidator? Guard { get; private set; }
    
    public IReadOnlySet<EntityTag> Tags => _tags;
    public IReadOnlyList<TriggerEntry> Triggers => _triggers;
    public IReadOnlyList<IActionDefinition> Actions => _actions;
    public IReadOnlyList<EffectDefinitionId> Effects => _effects;
    public bool IsTriggered => _triggers.Count > 0;
    
    private PassiveTrigger(){}
    public static PassiveTrigger Create() => new();
    
    public void SetShape(ITargetShape? shape) => Shape = shape;
    public void SetFilter(ITargetFilter? filter) => Filter = filter;
    public void SetGuard(IValidator? guard) => Guard = guard;
    
    public void AddTag(EntityTag tag) => _tags.Add(tag);
    public void RemoveTag(EntityTag tag) => _tags.Remove(tag);
    public void ClearTags() => _tags.Clear();
    
    public void AddTrigger(TriggerEntry trigger)
    {
        ArgumentNullException.ThrowIfNull(trigger);
        _triggers.Add(trigger);
    }
    public void RemoveTrigger(TriggerEntry trigger) => _triggers.Remove(trigger);
    
    public void AddEffect(EffectDefinitionId effectId)
    {
        ArgumentNullException.ThrowIfNull(effectId);
        _effects.Add(effectId);
    }
    public void RemoveEffect(EffectDefinitionId effectId) => _effects.Remove(effectId);
    public void ClearEffects() => _effects.Clear();
    
    public void AddAction(IActionDefinition action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _actions.Add(action);
    }
    public void RemoveAction(IActionDefinition action) => _actions.Remove(action);
    public void ClearActions() => _actions.Clear();
}