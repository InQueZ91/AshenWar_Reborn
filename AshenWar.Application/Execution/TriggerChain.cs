using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Constants;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Application.Translation;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Events.Units;
using AshenWar.Domain.Interfaces;

namespace AshenWar.Application.Execution;

public sealed class TriggerChain(
    PassiveTriggerExecutor passiveTriggerExecutor,
    ResolutionEventTranslatorRegistry translatorRegistry)
{
    public async Task<ResolutionBatch> Run(IDomainEventCollector collector,
        MatchContext matchContext,
        CancellationToken cancellationToken)
    {
        var tickNumber = 0;
        var resolutionTicks = new List<ResolutionTick>();
        
        while (collector.HasEvents && tickNumber < ResolutionConstants.MaxPassiveChainDepth)
        {
            tickNumber++;
            var snapshot = collector.Flush();
            
            // Phase 1 - Consequences (mandatory, not passive triggers)
            // Despawn dead units and collect the despawn events into this tick
            var consequenceEvents = new List<IDomainEvent>();
            foreach (var evt in snapshot.OfType<UnitDied>())
            {
                var unit = matchContext.Board.FindUnitById(evt.UnitId);
                if (unit is null) continue;
                
                // Dead unit's own death triggers fire before despawn
                var matching = matchContext.TriggerRegistry.GetMatching(evt, matchContext.Board);
                foreach (var registration in matching.Where(r => r.Owner == unit))
                    await passiveTriggerExecutor.Execute(evt,
                        registration,
                        matchContext,
                        collector,
                        cancellationToken);
                
                // Despawn - this raises UnitDespawned into collector
                matchContext.Match.DespawnUnit(unit);
                
                // Capture the despawn event into THIS tick, not the next
                consequenceEvents.AddRange(collector.Flush());
            }
            
            // Phase 2 - Reactions to everything in this tick
            var allThisTick = snapshot
                .Where(e => e is not UnitDied)
                .Concat(consequenceEvents)
                .ToList();

            foreach (var evt in allThisTick)
            {
                var matching = matchContext.TriggerRegistry.GetMatching(evt, matchContext.Board);
                foreach (var registration in matching)
                    await passiveTriggerExecutor.Execute(evt,
                        registration,
                        matchContext,
                        collector,
                        cancellationToken);
            }
            
            var finalSnapshot = translatorRegistry.TranslateAll(snapshot.Concat(consequenceEvents));
            resolutionTicks.Add(new ResolutionTick(tickNumber, finalSnapshot));
        }

        return new ResolutionBatch(resolutionTicks.ToImmutableList());
    }
}