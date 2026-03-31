using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities.Units;
using Domain.Enums;
using Domain.Interfaces.Effect;
using Domain.Interfaces.Modifier;
using Domain.Interfaces.Object;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

namespace Domain.Entities.Abilities;

public class Ability : Entity, IStatHolder<AbilityStat>
{
    #region Identifiers and References
    
    public AbilityId Id { get; } = AbilityId.New();
    public string Name { get; private set; }

    public Unit Owner { get; private set; }
    public AbilityDefinitionId DefinitionId { get; private set; }
    
    #endregion
    
    public List<IEffect> Effects { get; }  
    // AbilitySteps
    
    #region Stats

    public AbilityStats BaseStats { get; }
    public int Cost { get; private set; }
    public int Cooldown { get; private set; }
    public int Range { get; private set; }

    #endregion

    // Constructor
    private Ability(AbilityDefinition definition, Unit owner)
    {
        Owner = owner;
        DefinitionId = definition.Id;
        Name = definition.Name;
        
        Effects = definition.Effects;
        
        BaseStats = definition.Stats;
        Cost = BaseStats.Cost;
        Cooldown = BaseStats.Cooldown;
        Range = BaseStats.Range;
    }
    public static Ability Instantiate(AbilityDefinition definition, Unit owner)
    {
        return new Ability(definition, owner);
    }

    // Methods
    public void Execute()
    {
        // var targets = TargetingStrategy.SelectTargets(targetContext);
        //
        // // Apply effects to all targets
        // foreach (var target in targets)
        // {
        //     foreach (var effect in Effects)
        //     {
        //         var effectContext = new EffectContext(
        //             targetContext.Source
        //             ,target
        //             ,
        //             ,);
        //         
        //         effect.Apply(effectContext);
        //     }
        // }
    }

    #region StatHolder
    
    public int GetStat(AbilityStat stat)
    {
        var modifiers = Owner.StatusEffects
            .OfType<IModifierProvider<IAbilityStatModifier>>()
            .SelectMany(s => s.GetModifiers())
            .Where(m => m.AbilityStat == stat);
        
        var baseValue = GetBaseStat(stat);
        
        float flat = 0;
        float additive = 0;
        float multiplicative = 1;

        foreach (var m in modifiers)
        {
            switch (m.ModifierType)
            {
                case ModifierType.Flat:
                    flat += m.Modifier;
                    break;
                case ModifierType.Additive:
                    additive += m.Modifier;
                    break;
                case ModifierType.Multiplicative:
                    multiplicative *= m.Modifier;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        var final = (baseValue + flat) * (1 + additive) * multiplicative;
        
        return (int) Math.Round(final);
    }
    public int GetBaseStat(AbilityStat stat)
    {
        return stat switch
        {
            AbilityStat.Cost => BaseStats.Cost,
            AbilityStat.Cooldown => BaseStats.Cooldown,
            AbilityStat.Range => BaseStats.Range,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }

    #endregion
}
