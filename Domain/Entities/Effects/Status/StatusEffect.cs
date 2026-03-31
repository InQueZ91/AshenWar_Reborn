using System;
using System.Collections.Generic;
using Domain.Interfaces.Effect;
using Domain.Interfaces.Modifier;

namespace Domain.Entities.Effects.Status;

public abstract class StatusEffect
{
    public Guid Id { get; } = Guid.NewGuid(); 
    private int _remainingTurns;
    protected StatusEffect(int duration)
    {
        _remainingTurns = duration;
    }
    
    public virtual void OnApply(IEffectTarget target) {}
    public virtual void OnTurnStart(IEffectTarget target) {}
    public virtual void OnExpire(IEffectTarget target) {}
    public bool Tick(IEffectTarget target)
    {
        OnTurnStart(target);
        
        _remainingTurns--;
        
        return _remainingTurns <= 0;
    }
}