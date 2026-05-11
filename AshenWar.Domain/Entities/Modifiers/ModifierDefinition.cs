using System.Collections.Generic;
using Domain.Interfaces;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Actions;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;
using Domain.ValueObjects.LocalIdentifiers;

namespace Domain.Entities.Modifiers;

public abstract class ModifierDefinition(
    IOperation operation,
    float baseValue,
    float perStackValue,
    IStackScaling scaling,
    StatDefinitionId statId,
    HashSet<EntityTag> scopeFilter,
    IValidator? guard = null)
{
    public ModifierDefinitionId Id { get; } = ModifierDefinitionId.New();
    public IOperation Operation => operation;
    public float BaseValue => baseValue;
    public float PerStackValue => perStackValue;
    public IStackScaling Scaling => scaling;
    public StatDefinitionId StatId => statId;
    public HashSet<EntityTag> ScopeFilter => scopeFilter;
    public IValidator? Guard => guard;

    public float CalculateValue(int stacks) => scaling.Calculate(baseValue, perStackValue, stacks);
}