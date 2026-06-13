using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Conditions;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;

namespace AshenWar.Application.Execution;

public sealed class ConditionExecutor<TCondition>(TriggerChain triggerChain, ActionExecutor actionExecutor) 
    where TCondition : ConditionBase
{
    public async Task Apply(
        TCondition condition,
        ICondition<TCondition> holder,
        MatchContext matchContext,
        IDomainEventCollector collector, // collector passed in, not created here
        CancellationToken ct)
    {
        var candidates = condition.Definition
            .ResolveCandidates(holder as ITargetable, matchContext.Board);

        await ExecuteConditionEffects(
            condition.Definition.Tags,
            condition.Definition.OnApply,
            candidates,
            holder as ITargetable,
            matchContext,
            collector, // same collector, events accumulate
            ct
        );
        
        // no trigger chain here, caller owns it
    }
    
    public async Task<ResolutionBatch> Tick(
        ICondition<TCondition> holder,
        MatchContext matchContext,
        CancellationToken ct)
    {
        var collector = new DomainEventCollector();
        
        foreach (var condition in holder.Conditions)
        {
            // Tick conditions
            condition.Tick();
            
            var candidates = condition.Definition
                .ResolveCandidates(holder as ITargetable, matchContext.Board);

            await ExecuteConditionEffects(
                condition.Definition.Tags,
                condition.Definition.OnTick,
                candidates,
                holder as ITargetable,
                matchContext,
                collector,
                ct
            );
            
            // Expire conditions
            if (!condition.IsExpired) continue;
            
            await ExecuteConditionEffects(
                condition.Definition.Tags,
                condition.Definition.OnExpire,
                candidates,
                holder as ITargetable,
                matchContext,
                collector,
                ct
            );
                
            holder.RemoveCondition(condition.Id);
        }
        
        return await triggerChain.Run(collector, matchContext, ct);
    }

    private async Task ExecuteConditionEffects(
        IReadOnlySet<EntityTag> tags,
        IReadOnlyList<ConditionEffect> effects,
        IReadOnlyList<ITargetable> candidates,
        ITargetable? source,
        MatchContext matchContext,
        IDomainEventCollector collector,
        CancellationToken ct)
    {
        if (source is null) // Holder is match not unit or tile
            return;
        
        foreach (var effect in effects)
        {
            ct.ThrowIfCancellationRequested();
            
            var targets = (effect.Filter?.Apply(candidates, source) ?? candidates).ToImmutableList();
            var context = new ActionContext(source, targets, tags, matchContext);

            if (effect.Guard?.Check(context) == false) continue;

            foreach (var action in effect.Actions)
                await actionExecutor.Execute(action, context, collector, ct);
        }
    }
}