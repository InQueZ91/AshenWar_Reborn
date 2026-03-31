using Domain.Enums;
using Domain.Interfaces.Modifier;

namespace Domain.Entities.Effects.Modifiers;

public class AbilityStatModifier : IAbilityStatModifier
{
    public AbilityStat AbilityStat { get; }
    public float Modifier { get; }
    public ModifierType ModifierType { get; }
    
    public AbilityStatModifier(AbilityStat abilityStat, ModifierType modifierType, float modifier)
    {
        AbilityStat = abilityStat;
        ModifierType = modifierType;
        Modifier = modifier;
    }
}