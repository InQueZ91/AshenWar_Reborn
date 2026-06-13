using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Application.Contracts.Execution;

public interface IDomainEventCollector
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    bool HasEvents => DomainEvents.Any();
    void Collect(IDomainEvent domainEvent);
    IReadOnlyList<IDomainEvent> Flush();
}