using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Ability;
using AshenWar.Domain.Interfaces.Abilities;

namespace AshenWar.Application.Execution.Handlers.Ability;

public sealed class RemovePassiveHandler : IActionHandler<RemovePassive>
{
    public Task Execute(RemovePassive definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        if (context.Source is not IPassive holder)
            return Task.CompletedTask;
        
        // Fetch passive
        var passive = holder.GetPassiveByDefinitionId(definition.PassiveDefinitionId);
        if (passive is null)
            return Task.CompletedTask;
        
        // Unregister passive from trigger registry
        context.MatchContext.TriggerRegistry.Unregister(context.Source, passive);
        
        // Remove passive
        holder.RemovePassive(passive.Id);
        
        return Task.CompletedTask;
    }
}