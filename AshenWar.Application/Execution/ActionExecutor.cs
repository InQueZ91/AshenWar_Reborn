using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Application.Execution;

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
        var (handler, scope) = registry.Get<T>();
        using (scope)
        {
            await handler.Execute(action, context, collector, cancellationToken);
        }

        // Drain source
        if (context.Source is DomainEntity sourceEntity)
            foreach (var evt in sourceEntity.DrainDomainEvents())
                collector.Collect(evt);

        // Drain targets
        foreach (var target in context.Targets.OfType<DomainEntity>())
        foreach (var evt in target.DrainDomainEvents())
            collector.Collect(evt);
    }
}