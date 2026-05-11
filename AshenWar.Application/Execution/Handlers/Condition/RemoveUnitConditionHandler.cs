using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Condition;
using Domain.Entities.Conditions.Unit;
using Domain.Interfaces.Conditions;

namespace Application.Execution.Handlers.Condition;

public sealed class RemoveUnitConditionHandler : IActionHandler<RemoveUnitCondition>
{
    public Task Execute(RemoveUnitCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        foreach (var target in context.Targets)
        {
            if (target is not IConditionCommand<UnitCondition> holder) continue;

            var condition = holder.Conditions
                .FirstOrDefault(c => c.Definition.Id == definition.UnitConditionDefinitionId);

            if (condition is null) continue;

            holder.RemoveCondition(condition.Id);
        }

        return Task.CompletedTask;
    }
}