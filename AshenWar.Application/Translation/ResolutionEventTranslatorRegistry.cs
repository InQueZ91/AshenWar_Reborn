using System;
using System.Collections.Generic;
using System.Linq;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Application.Translation;

public sealed class ResolutionEventTranslatorRegistry
{
    private readonly Dictionary<Type, Func<IDomainEvent, IReadOnlyList<ResolutionEvent>>> _map = new();

    public void Register<T>(IResolutionEventTranslator<T> translator) where T : IDomainEvent
    {
        _map[typeof(T)] = e => translator.Translate((T) e);
    }

    public IReadOnlyList<ResolutionEvent> Translate(IDomainEvent domainEvent)
    {
        return _map.TryGetValue(domainEvent.GetType(), out var translate)
            ? translate(domainEvent)
            : []; // no translator registered = discard
    }

    public IReadOnlyList<ResolutionEvent> TranslateAll(IEnumerable<IDomainEvent> domainEvents)
    {
        return domainEvents.SelectMany(Translate).ToList();
    }
}