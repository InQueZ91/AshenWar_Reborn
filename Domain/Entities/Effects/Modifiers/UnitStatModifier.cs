using System;
using Domain.Entities.Units;
using Domain.Enums;
using Domain.Interfaces.Modifier;

namespace Domain.Entities.Effects.Modifiers;

public class UnitStatModifier : IUnitStatModifier
{
    public UnitStat UnitStat { get; }
    public ModifierType ModifierType { get; }
    public float Modifier { get; }

    public UnitStatModifier(UnitStat unitStat, ModifierType modifierType, float modifier)
    {
        UnitStat = unitStat;
        ModifierType = modifierType;
        Modifier = modifier;
    }
}