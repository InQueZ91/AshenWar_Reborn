using System.Collections.Generic;
using Domain.Entities.Effects.Modifiers;
using Domain.Enums;
using Domain.Interfaces.Effect;
using Domain.Interfaces.Modifier;
using Domain.Interfaces.Object;

namespace Domain.Entities.Effects.Status;

public class Burn : StatusEffect, IModifierProvider<IDamageModifier>
{
    private readonly int _damagePerTick;
    private readonly IDamageModifier[] _damageModifiers;
    
    public Burn(int duration, int damagePerTick, float fireGain) : base(duration)
    {
        _damagePerTick = damagePerTick;
        _damageModifiers = new IDamageModifier[]
        {
            new DamageModifier(DamageType.Fire, ModifierType.Multiplicative, fireGain)
        };
    }

    public override void OnTurnStart(IEffectTarget target)
    {
        if (target is IDamageable damageable)
            damageable.TakeDamage(_damagePerTick);
    }

    public IEnumerable<IDamageModifier> GetModifiers() => _damageModifiers;
}