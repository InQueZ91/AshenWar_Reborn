using System;
using AshenWar.Domain.Events;
using AshenWar.Domain.Exceptions;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Domain.Entities.Abilities;

public sealed class Ability : DomainEntity
{
    public AbilityId Id { get; private set;}
    public AbilityDefinition Definition { get; }
    public int RemainingCooldown { get; private set;}
    public bool IsReady => RemainingCooldown == 0;

    private Ability(AbilityId id, AbilityDefinition definition, int remainingCooldown)
    {
        Id = id;
        Definition = definition;
        RemainingCooldown = remainingCooldown;
    }
    public static Ability Instantiate(AbilityDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        
        return new Ability(AbilityId.New(), definition, 0);
    }
    public static Ability Rehydrate(AbilityId id, AbilityDefinition definition, int remainingCooldown)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(definition);

        if (remainingCooldown < 0)
            throw new DomainException("Remaining cooldown cannot be negative.");
        
        return new Ability(id, definition, remainingCooldown);
    }
    
    public void SetCooldown(int value)
    {
        RemainingCooldown = Math.Max(0, value);
        
        RaiseDomainEvent(new AbilityCooldownChanged(Id, RemainingCooldown));
    }
    public void TickCooldown()
    {
        if (IsReady) return;
        SetCooldown(RemainingCooldown - 1);
    }
}
