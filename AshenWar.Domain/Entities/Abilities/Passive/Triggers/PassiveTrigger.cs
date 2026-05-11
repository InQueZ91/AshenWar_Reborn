using System;
using System.Collections.Generic;
using Domain.Interfaces;
using Domain.Interfaces.Abilities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers.Effects;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Entities.Abilities.Passive.Triggers;

public sealed class PassiveTrigger
{
    private readonly HashSet<EntityTag> _tags = [];
    private readonly List<TriggerEntry> _triggers = [];
    private readonly List<EffectDefinitionId> _effects = [];
    
    public PassiveTriggerId Id { get; } = PassiveTriggerId.New();
    public ITargetShape? Shape { get; private set; }
    public ITargetFilter? Filter { get; private set; }
    public IValidator? Guard { get; private set; }
    
    public IReadOnlySet<EntityTag> Tags => _tags;
    public IReadOnlyList<TriggerEntry> Triggers => _triggers;
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
    
    public void AddTrigger<TEvent>(Func<IDomainEvent, bool>? predicate) where TEvent : IDomainEvent
    {
        _triggers.Add(TriggerEntry.ForType<TEvent>(predicate));
    }
    public void RemoveTrigger(TriggerEntry trigger) => _triggers.Remove(trigger);
    
    public void AddEffect(EffectDefinitionId effectId) => _effects.Add(effectId);
    public void RemoveEffect(EffectDefinitionId effectId) => _effects.Remove(effectId);
    public void ClearEffects() => _effects.Clear();
}