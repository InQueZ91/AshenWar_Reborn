using Domain.Enums;
using Domain.Interfaces.Modifier;

namespace Domain.Entities.Effects.Modifiers;

public class DamageModifier : IDamageModifier
{
    public DamageType DamageType { get; }
    public float Modifier { get; }
    public ModifierType ModifierType { get; }
    
    public DamageModifier(DamageType damageType, ModifierType modifierType, float modifier)
    {
        DamageType = damageType;
        ModifierType = modifierType;
        Modifier = modifier;
    }

}