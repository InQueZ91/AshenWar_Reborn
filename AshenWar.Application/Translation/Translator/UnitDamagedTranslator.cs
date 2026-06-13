using System.Collections.Generic;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Events.Units;

namespace AshenWar.Application.Translation.Translator;

public sealed class UnitDamagedTranslator : IResolutionEventTranslator<UnitDamaged>
{
    public IReadOnlyList<ResolutionEvent> Translate(UnitDamaged e) 
        => [new ResolutionEvents.UnitDamaged(e.UnitId.Value, e.Amount, e.RemainingHealth)];
}