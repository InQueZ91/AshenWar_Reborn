using System.Collections.Generic;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Entities.Abilities;

public sealed class AbilityStep
{
    private readonly List<EffectDefinitionId> _effects = [];
    private readonly List<IActionDefinition> _actions = [];
    
    public AbilityStepId Id { get; } = AbilityStepId.New();
    public IValidator? Validator { get; private set; }
    public ITargetShape Shape { get; private set; }
    public ITargetFilter? Filter { get; private set; }

    public IReadOnlyList<IActionDefinition> Actions => _actions;
    public IReadOnlyList<EffectDefinitionId> Effects => _effects;

    private AbilityStep(ITargetShape shape)
    {
        Shape = shape;
    }
    public static AbilityStep Create(ITargetShape shape) => new(shape);
    
    public void SetValidator(IValidator validator) => Validator = validator;
    public void SetShape(ITargetShape shape) => Shape = shape;
    public void SetFilter(ITargetFilter filter) => Filter = filter;
    
    public void AddEffect(EffectDefinitionId effectId) => _effects.Add(effectId);
    public void RemoveEffect(EffectDefinitionId effectId) => _effects.RemoveAll(e => e == effectId);
    public void ClearEffects() => _effects.Clear();
    
    public void AddAction(IActionDefinition action) => _actions.Add(action);
    public void RemoveAction(IActionDefinition action) => _actions.RemoveAll(a => a == action);
    public void ClearActions() => _actions.Clear();
}