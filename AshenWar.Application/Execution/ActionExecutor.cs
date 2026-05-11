using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Actions;
using Domain.Interfaces.Actions;

namespace Application.Execution;

public sealed class ActionExecutor(ActionHandlerRegistry registry)
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> DispatchCache = new();

    public async Task Execute(
        IActionDefinition action,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var method = DispatchCache.GetOrAdd(action.GetType(), t =>
            typeof(ActionExecutor)
                .GetMethod(nameof(Dispatch), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(t));

        await (Task)method.Invoke(this, [action, context, collector, cancellationToken])!;
    }

    private async Task Dispatch<T>(T action,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken) where T : IActionDefinition
    {
        await registry.Get<T>().Execute(action, context, collector, cancellationToken);
        
        // Drain source
        if (context.Source is MatchEntity sourceEntity)
            foreach (var evt in sourceEntity.FlushDomainEvents())
                collector.Collect(evt);
        
        // Drain targets
        foreach (var target in context.Targets.OfType<MatchEntity>())
            foreach (var evt in target.FlushDomainEvents())
                collector.Collect(evt);
    }
}