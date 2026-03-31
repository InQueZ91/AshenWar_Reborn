using Domain.Entities.Effects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Interfaces.Effect;

public interface IEffect
{
    EffectId Id { get; }
    void Apply(EffectContext ctx);
}