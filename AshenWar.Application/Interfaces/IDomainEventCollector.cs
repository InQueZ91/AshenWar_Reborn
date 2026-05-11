using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IDomainEventCollector
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    bool HasEvents => DomainEvents.Any();
    void Collect(IDomainEvent domainEvent);
    IReadOnlyList<IDomainEvent> Flush();
}