using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Condition;
using Domain.Entities.Conditions.Unit;
using Domain.Interfaces.Conditions;

namespace Application.Execution.Handlers.Condition;

public sealed class ApplyUnitConditionHandler(
    IConditionRepository conditionRepository,
    ConditionExecutor<UnitCondition> conditionExecutor)
    : IActionHandler<ApplyUnitCondition>
{
    public async Task Execute(ApplyUnitCondition definition, ActionContext context, IDomainEventCollector collector, CancellationToken cancellationToken)
    {
        var conditionDefinition = await conditionRepository.GetUnitConditionAsync(definition.UnitConditionDefinitionId,
            cancellationToken);
        
        var stacksToApply = (int)definition.Stacks.Resolve(context);

        var filteredTarget = conditionDefinition.Filter?.Apply(context.Targets, context.Source) 
                             ?? context.Targets;
        
        foreach (var target in filteredTarget)
        {
            if (target is not IConditionCommand<UnitCondition> holder)
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