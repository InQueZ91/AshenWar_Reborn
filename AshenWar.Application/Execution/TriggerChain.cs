using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Constants;
using Application.Interfaces;
using Application.ValueObjects;
using Domain.Entities.Match;
using Domain.Events.Units;

namespace Application.Execution;

public sealed class TriggerChain(PassiveTriggerExecutor passiveTriggerExecutor)
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
            resolutionTicks.Add(new ResolutionTick(tickNumber,
                snapshot));

            foreach (var evt in snapshot.OfType<UnitDied>())
            {
                var unit = matchContext.Match.GetUnitById(evt.UnitId);
                if (unit is not null)
                {
                    var matching = matchContext.TriggerRegistry.GetMatching(evt);
                    
                    // Dead unit's own passives first
                    foreach (var registration in matching.Where(r => r.Owner == unit))
                        await passiveTriggerExecutor.Execute(evt,
                            registration,
                            matchContext,
                            collector,
                            cancellationToken);
                    
                    // Then despawn unit
                    matchContext.Match.DespawnUnit(unit);
                    
                    // Other units react after despawn
                    foreach (var registration in matching.Where(r => r.Owner != unit))
                        await passiveTriggerExecutor.Execute(evt,
                            registration,
                            matchContext,
                            collector,
                            cancellationToken);
                }
            }

            foreach (var evt in snapshot.Where(e => e is not UnitDied))
            {
                var matching = matchContext.TriggerRegistry.GetMatching(evt);
                foreach (var registration in matching) 
                    await passiveTriggerExecutor.Execute(evt,
                        registration,
                        matchContext,
                        collector, 
                        cancellationToken);
            }
        }

        return new ResolutionBatch(resolutionTicks.ToImmutableList());
    }
}