using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;
using AshenWar.Domain.Entities.Conditions.Unit;
using AshenWar.Domain.Interfaces.Conditions;

namespace AshenWar.Application.Execution.Handlers.Condition;

public sealed class ApplyUnitConditionHandler(
    IConditionDefinitionRepository conditionDefinitionRepository,
    ConditionExecutor<UnitCondition> conditionExecutor)
    : IActionHandler<ApplyUnitCondition>
{
    public async Task Execute(ApplyUnitCondition definition, ActionContext context, IDomainEventCollector collector, CancellationToken cancellationToken)
    {
        var conditionDefinition = await conditionDefinitionRepository.GetUnitConditionAsync(definition.UnitConditionDefinitionId,
            cancellationToken);
        
        var stacksToApply = (int)definition.Stacks.Resolve(context);

        var filteredTarget = conditionDefinition.Filter?.Apply(context.Targets, context.Source) 
                             ?? context.Targets;
        
        foreach (var target in filteredTarget)
        {
            if (target is not ICondition<UnitCondition> holder)
                continue;
            
            var targetContext = context with { Targets = [target] };
            if (conditionDefinition.Guard?.Check(targetContext) == false)
                continue;
            
            var condition = UnitCondition.Instantiate(conditionDefinition, stacksToApply);
            holder.ApplyCondition(condition);
            
            await conditionExecutor.Apply(condition, holder, context.MatchContext, collector, cancellationToken);
        }
    }
}