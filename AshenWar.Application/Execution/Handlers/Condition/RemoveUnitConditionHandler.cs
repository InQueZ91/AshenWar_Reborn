using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Application.Execution.Handlers.Condition;

public sealed class RemoveUnitConditionHandler : IActionHandler<RemoveUnitCondition>
{
    public Task Execute(RemoveUnitCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        foreach (var target in context.Targets)
        {
            if (target is not ICondition<UnitCondition> holder) continue;

            var condition = holder.Conditions
                .FirstOrDefault(c => c.Definition.Id == definition.UnitConditionDefinitionId);

            if (condition is null) continue;

            holder.RemoveCondition(condition.Id);
        }

        return Task.CompletedTask;
    }
}