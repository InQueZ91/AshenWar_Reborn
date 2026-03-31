using System.Collections.Generic;
using Domain.Entities.Effects.Modifiers;
using Domain.Enums;
using Domain.Interfaces.Effect;
using Domain.Interfaces.Modifier;
namespace Domain.Entities.Effects.Status;

public class Haste : StatusEffect, IModifierProvider<IUnitStatModifier>, IModifierProvider<IAbilityStatModifier>
{
    private readonly IUnitStatModifier[] _unitStatModifiers;
    private readonly IAbilityStatModifier[] _abilityStatModifiers;
    
    public Haste(int duration, float speedGain, float stepGain, float cooldownGain) : base(duration)
    {
        _unitStatModifiers = new IUnitStatModifier[]
        {
            new UnitStatModifier(UnitStat.Speed, ModifierType.Multiplicative, speedGain),
            new UnitStatModifier(UnitStat.Steps, ModifierType.Multiplicative, stepGain)
        };
        
        _abilityStatModifiers = new IAbilityStatModifier[]
        {
            new AbilityStatModifier(AbilityStat.Cooldown, ModifierType.Multiplicative, cooldownGain)
        };
    }
    public IEnumerable<IUnitStatModifier> GetModifiers() => _unitStatModifiers;
    IEnumerable<IAbilityStatModifier> IModifierProvider<IAbilityStatModifier>.GetModifiers() => _abilityStatModifiers;
}
