using System;
using System.Collections.Generic;
using Domain.Interfaces;

namespace Domain.Entities
{
    public abstract class MatchEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);
            _domainEvents.Add(domainEvent);
        }
        public IReadOnlyList<IDomainEvent> FlushDomainEvents()
        {
            var events = _domainEvents.AsReadOnly();
            _domainEvents.Clear();
            return events;
        }
    }
}