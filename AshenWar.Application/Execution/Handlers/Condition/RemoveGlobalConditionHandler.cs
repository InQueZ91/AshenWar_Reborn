using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;

namespace AshenWar.Application.Execution.Handlers.Condition;

public sealed class RemoveGlobalConditionHandler : IActionHandler<RemoveGlobalCondition>
{
    public Task Execute(RemoveGlobalCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        var holder = context.MatchContext.Match;
        
        var condition = holder.Conditions
            .FirstOrDefault(c => c.Definition.Id == definition.GlobalConditionDefinitionId);

        if (condition is null) return Task.CompletedTask;
        
        holder.RemoveCondition(condition.Id);
        
        return Task.CompletedTask;
    }
}