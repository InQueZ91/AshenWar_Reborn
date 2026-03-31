using Domain.Enums;
using Domain.Interfaces.Effect;

namespace Domain.Entities.Effects;

public sealed class EffectContext
{
    public IEffectTarget Caster { get; set; }
    public IEffectTarget Target { get; set; }

    public float Value { get; set; }
    public DamageType DamageType { get; set; }
    
    // ability intent;
    
    public EffectContext(
        IEffectTarget caster,
        IEffectTarget target,
        float value,
        DamageType damageType)
    {
        Caster = caster;
        Target = target;
        Value = value;
        DamageType = damageType;
    }
}