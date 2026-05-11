using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Condition;
using Domain.Entities.Conditions.Tile;
using Domain.Interfaces.Conditions;

namespace Application.Execution.Handlers.Condition;

public sealed class RemoveTileConditionHandler : IActionHandler<RemoveTileCondition>
{
    public Task Execute(RemoveTileCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        foreach (var target in context.Targets)
        {
            if (target is not IConditionCommand<TileCondition> holder) continue;

            var condition = holder.Conditions
                .FirstOrDefault(c => c.Definition.Id == definition.TileConditionDefinitionId);

            if (condition is null) continue;

            holder.RemoveCondition(condition.Id);
        }

        return Task.CompletedTask;
    }
}