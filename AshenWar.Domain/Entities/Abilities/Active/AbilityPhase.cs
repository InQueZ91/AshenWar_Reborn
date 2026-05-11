using System.Collections.Generic;
using Domain.Interfaces.Abilities;
using Domain.ValueObjects.Identifiers.Effects;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Entities.Abilities.Active;

public sealed class AbilityPhase
{
    public AbilityPhaseId Id { get; } = AbilityPhaseId.New();
    public IValidator? Validator { get; private set; }
    public ITargetShape? Shape { get; private set; }
    public ITargetFilter? Filter { get; private set; }
    
    private readonly List<EffectDefinitionId> _effects = [];
    public IReadOnlyList<EffectDefinitionId> Effects => _effects;
    
    private AbilityPhase() {}
    public static AbilityPhase Create() => new();
    
    public void SetGuard(IValidator guard) => Validator = guard;
    public void SetShape(ITargetShape shape) => Shape = shape;
    public void SetFilter(ITargetFilter filter) => Filter = filter;
    
    public void AddEffect(EffectDefinitionId effectId) => _effects.Add(effectId);
    public void RemoveEffect(EffectDefinitionId effectId) => _effects.RemoveAll(e => e == effectId);
    public void ClearEffects() => _effects.Clear();
}