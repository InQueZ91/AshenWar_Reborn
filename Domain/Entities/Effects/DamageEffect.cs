using Domain.Interfaces.Effect;
using Domain.Interfaces.Object;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Effects;

public sealed class DamageEffect : IEffect
{
    public EffectId Id { get; } = EffectId.New();
    
    private readonly int _amount;

    public DamageEffect(int amount)
    {
        _amount = amount;
    }

    public void Apply(EffectContext ctx)
    {
        if (ctx.Target is IDamageable damageable)
            damageable.TakeDamage(_amount);
    }
}