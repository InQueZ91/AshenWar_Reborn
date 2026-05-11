using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Ability;
using Domain.Interfaces.Abilities.Passive;

namespace Application.Execution.Handlers.Ability;

public sealed class RemovePassiveHandler : IActionHandler<RemovePassive>
{
    public Task Execute(RemovePassive definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        if (context.Source is not IPassiveAbilityCommand holder)
            return Task.CompletedTask;
        
        // Fetch passive
        var passive = holder.GetPassiveByDefinitionId(definition.PassiveAbilityDefinitionId);
        if (passive is null)
            return Task.CompletedTask;
        
        // Unregister passive from trigger registry
        context.MatchContext.TriggerRegistry.Unregister(context.Source, passive);
        
        // Remove passive
        holder.RemovePassive(passive.Id);
        
        return Task.CompletedTask;
    }
}