using System;
using Domain.Interfaces;

namespace Domain.Entities.Abilities.Passive.Triggers;

public sealed class TriggerEntry
{
    public Type EventType { get; }
    public Func<IDomainEvent, bool>? Predicate { get; }
    
    private TriggerEntry(Type eventType, Func<IDomainEvent, bool>? predicate)
    {
        EventType = eventType;
        Predicate = predicate;
    }

    public static TriggerEntry ForType<TEvent>() where TEvent : IDomainEvent
        => new (typeof(TEvent), null);
    
    public static TriggerEntry ForType<TEvent>(Func<IDomainEvent, bool>? predicate) where TEvent : IDomainEvent
        => new (typeof(TEvent), predicate);
    
    public bool Matches(IDomainEvent e)
    => EventType.IsInstanceOfType(e) && (Predicate?.Invoke(e) ?? true);
}