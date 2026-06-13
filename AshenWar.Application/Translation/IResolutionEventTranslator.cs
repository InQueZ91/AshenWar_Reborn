using System.Collections.Generic;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Application.Translation;

public interface IResolutionEventTranslator<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    IReadOnlyList<ResolutionEvent> Translate(TDomainEvent domainEvent);
}