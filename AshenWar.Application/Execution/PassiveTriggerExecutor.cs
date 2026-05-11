using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Abilities.Passive.Triggers;
using Domain.Entities.Actions;
using Domain.Entities.Match;
using Domain.Interfaces;
using Domain.Interfaces.Entities;

namespace Application.Execution;

public sealed class PassiveTriggerExecutor(EffectExecutor effectExecutor)
{
    public async Task Execute(
        IDomainEvent triggerEvent,
        TriggerRegistration triggerRegistration,
        MatchContext matchContext,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var source = triggerRegistration.Owner;
        var triggerEntry = triggerRegistration.Entry;
        var effectGroup = triggerRegistration.PassiveTrigger;
        
        // 1 Validate Trigger Conditions
        if (triggerEntry.Predicate != null && !triggerEntry.Predicate(triggerEvent))
        {
            // Might want to log this
            return;
        }
        
        // 2 Resolve Targets
        var shapeOrigin = triggerRegistration.AnchorPosition ?? source.Position;
        
        var candidates = effectGroup.Shape?
            .Resolve(shapeOrigin, source, matchContext.Board) ?? new List<ITargetable> { source };

        // 3 Filter targets
        var filtered = effectGroup.Filter?.Apply(candidates, source) ?? candidates;

        // 4 Build Action Context
        var actionContext = new ActionContext(source, filtered.ToList(), effectGroup.Tags, matchContext);

        // 5 Validate Guard Conditions
        if (effectGroup.Guard?.Check(actionContext) == false)
        {
            // Might want to log this
            return;
        }

        // 6 Execute Effects
        foreach (var effectDefinitionId in effectGroup.Effects) 
            await effectExecutor.Execute(effectDefinitionId, actionContext, collector, cancellationToken);
    }
}