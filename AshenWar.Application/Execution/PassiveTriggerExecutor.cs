using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Abilities.Passives.Triggers;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Entities;
using Microsoft.Extensions.Logging;

namespace AshenWar.Application.Execution;

public sealed partial class PassiveTriggerExecutor(
    ILogger<PassiveTriggerExecutor> logger,
    EffectExecutor effectExecutor,
    ActionExecutor actionExecutor)
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
        var passiveTrigger = triggerRegistration.PassiveTrigger;
        var board = matchContext.Board;
        
        // // 1 Validate Trigger Conditions
        if (!triggerEntry.Matches(triggerEvent, source, matchContext.Board))
        {
            LogMismatchedTriggerExecution(logger, triggerEntry.GetType().Name, triggerEvent.GetType().Name);
            return;
        }
        
        // 2 Resolve Targets
        var shapeOrigin = triggerRegistration.AnchorPosition ?? source.Position;
        if (shapeOrigin is null)
        {
            LogCouldNotResolveShapeOriginForPassiveTrigger(logger);
            return;
        }
        
        var candidates = passiveTrigger.Shape?
            .Resolve(shapeOrigin, source, matchContext.Board) ?? new List<ITargetable> { source };

        // 3 Filter targets
        var filtered = passiveTrigger.Filter?.Apply(candidates, source) ?? candidates;

        // 4 Build Action Context
        var actionContext = new ActionContext(source, filtered.ToList(), passiveTrigger.Tags, matchContext);

        // 5 Validate Guard Conditions
        if (passiveTrigger.Guard?.Check(actionContext) == false)
        {
            // Might want to log this
            return;
        }

        // 6 Execute Actions
        foreach (var action in passiveTrigger.Actions) 
            await actionExecutor.Execute(action, actionContext, collector, cancellationToken);
        
        // 7 Execute Effects
        foreach (var effectDefinitionId in passiveTrigger.Effects) 
            await effectExecutor.Execute(effectDefinitionId, actionContext, collector, cancellationToken);
    }

    [LoggerMessage(LogLevel.Warning, 
        "PassiveTriggerExecutor received mismatched registration: trigger {Trigger} cannot match event {Event}. This is a caller bug.")]
    static partial void LogMismatchedTriggerExecution(ILogger logger, string trigger, string @event);
    
    [LoggerMessage(LogLevel.Warning, "Could not resolve shape origin for passive trigger")]
    static partial void LogCouldNotResolveShapeOriginForPassiveTrigger(ILogger<PassiveTriggerExecutor> logger);
}