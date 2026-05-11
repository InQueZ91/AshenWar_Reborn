using System.Collections.Generic;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application;

public sealed class DomainEventCollector : IDomainEventCollector
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    
    public void Collect(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public IReadOnlyList<IDomainEvent> Flush()
    {
        var events = _domainEvents.AsReadOnly();
        _domainEvents.Clear();
        return events;
    }
}