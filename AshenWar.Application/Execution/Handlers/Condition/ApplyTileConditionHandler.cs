using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Condition;
using Domain.Entities.Conditions.Tile;
using Domain.Interfaces.Conditions;

namespace Application.Execution.Handlers.Condition;

public sealed class ApplyTileConditionHandler(
    IConditionRepository conditionRepository,
    ConditionExecutor<TileCondition> conditionExecutor)
    : IActionHandler<ApplyTileCondition>
{
    public async Task Execute(ApplyTileCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var conditionDefinition = await conditionRepository.GetTileConditionAsync(definition.TileConditionDefinitionId,
            cancellationToken);
        
        var stacksToApply = (int)definition.Stacks.Resolve(context);
        
        var filteredTarget = conditionDefinition.Filter?.Apply(context.Targets, context.Source) 
                             ?? context.Targets;
        
        foreach (var target in filteredTarget)
        {
            if (target is not IConditionCommand<TileCondition> holder)
                continue;
            
            var targetContext = context with { Targets = [target] };
            if (conditionDefinition.Guard?.Check(targetContext) == false)
                continue;
            
            var condition = TileCondition.Instantiate(conditionDefinition, stacksToApply);
            holder.ApplyCondition(condition);
            
            await conditionExecutor.Apply(condition, holder, context.MatchContext, collector, cancellationToken);
        }
    }
}