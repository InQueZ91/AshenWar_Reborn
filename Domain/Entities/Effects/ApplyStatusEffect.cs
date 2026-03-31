using System;
using Domain.Entities.Effects.Status;
using Domain.Interfaces.Effect;
using Domain.Interfaces.Object;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Effects;

public class ApplyStatusEffect : IEffect
{
    public EffectId Id { get; } = EffectId.New();
    
    private readonly Func<StatusEffect> _statusFactory;

    public ApplyStatusEffect(Func<StatusEffect> statusFactory)
    {
        _statusFactory = statusFactory;
    }


    public void Apply(EffectContext ctx)
    {
        if (ctx.Target is not IStatusHolder holder) return;
        
        var status = _statusFactory();
        holder.AddStatus(status);
    }
}