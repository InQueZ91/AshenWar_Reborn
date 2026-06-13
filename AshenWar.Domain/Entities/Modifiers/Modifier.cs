using System.Collections.Generic;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Actions;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.LocalIdentifiers;

namespace AshenWar.Domain.Entities.Modifiers;

public class Modifier(
    IOperation operation,
    IStackScaling scaling,
    StatDefinition stat,
    float baseValue,
    float perStackValue,
    HashSet<EntityTag> tagFilter,
    IValidator? guard = null)
{
    public ModifierDefinitionId Id { get; } = ModifierDefinitionId.New();
    public IOperation Operation => operation;
    public float BaseValue => baseValue;
    public float PerStackValue => perStackValue;
    public IStackScaling Scaling => scaling;
    public StatDefinition Stat => stat;
    public HashSet<EntityTag> TagFilter => tagFilter;
    public IValidator? Guard => guard;

    public float CalculateValue(int stacks) => scaling.Calculate(baseValue, perStackValue, stacks);
}