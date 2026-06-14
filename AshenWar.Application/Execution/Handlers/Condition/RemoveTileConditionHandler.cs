using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Application.Execution.Handlers.Condition;

public sealed class RemoveTileConditionHandler : IActionHandler<RemoveTileCondition>
{
    public Task Execute(RemoveTileCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        foreach (var target in context.Targets)
        {
            if (target is not ICondition<TileCondition> holder) continue;

            var condition = holder.Conditions
                .FirstOrDefault(c => c.Definition.Id == definition.TileConditionDefinitionId);

            if (condition is null) continue;

            holder.RemoveCondition(condition.Id);
        }

        return Task.CompletedTask;
    }
}