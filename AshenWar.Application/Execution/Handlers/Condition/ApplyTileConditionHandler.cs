using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;
using AshenWar.Domain.Entities.Conditions.Tile;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Application.Execution.Handlers.Condition;

public sealed class ApplyTileConditionHandler(
    IConditionDefinitionRepository conditionDefinitionRepository,
    ConditionExecutor<TileCondition> conditionExecutor)
    : IActionHandler<ApplyTileCondition>
{
    public async Task Execute(ApplyTileCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var conditionDefinition = await conditionDefinitionRepository.GetTileConditionAsync(definition.TileConditionDefinitionId,
            cancellationToken);
        
        var stacksToApply = (int)definition.Stacks.Resolve(context);
        
        var filteredTarget = conditionDefinition.Filter?.Apply(context.Targets, context.Source) 
                             ?? context.Targets;
        
        foreach (var target in filteredTarget)
        {
            if (target is not ICondition<TileCondition> holder)
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