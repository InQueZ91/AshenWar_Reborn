using System.Collections.Generic;
using Domain.Entities.Modifiers;
using Domain.Entities.Stats;
using Domain.Entities.Units;
using Domain.Interfaces.Entities;
using Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace Domain.Entities.Abilities.Active;

public sealed class ActiveAbility : MatchEntity, IStatHolder
{
    private readonly List<AbilityPhase> _abilityPhases = [];

    public ActiveAbilityId Id { get; } = ActiveAbilityId.New();
    public ActiveAbilityDefinition Definition { get; }
    public Unit Owner { get; private set; }
    public StatBlock BaseStats { get; }
    public int RemainingCooldown { get; private set;}
    public bool IsReady => RemainingCooldown <= 0;

    private ActiveAbility(ActiveAbilityDefinition definition, Unit owner)
    {
        Owner = owner;
        Definition = definition;
        BaseStats = definition.Stats;
        RemainingCooldown = 0;
    }
    
    public static ActiveAbility Instantiate(ActiveAbilityDefinition definition, Unit owner)
        => new (definition, owner);

    public void StartCooldown()
    {
        RemainingCooldown = GetMaxStat(StatDefinition.Cooldown);
    }
    public void TickCooldown()
    {
        if (RemainingCooldown > 0)
            RemainingCooldown--;
    }
 
    // IStatHolder
    public int GetMaxStat(StatDefinition stat) 
        => ModifierCalculator.Calculate(BaseStats.Get(stat), Owner.Conditions, stat.Id, Definition.Tags);
}
