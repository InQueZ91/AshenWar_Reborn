using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Condition;
using Domain.Entities.Conditions.Global;

namespace Application.Execution.Handlers.Condition;

public sealed class ApplyGlobalConditionHandler(
    IConditionRepository conditionRepository,
    ConditionExecutor<GlobalCondition> conditionExecutor)
    : IActionHandler<ApplyGlobalCondition>
{
    public async Task Execute(ApplyGlobalCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var conditionDefinition = await conditionRepository.GetGlobalConditionAsync(
            definition.GlobalConditionDefinitionId,
            cancellationToken);
        
        var stacksToApply = (int)definition.Stacks.Resolve(context);
        
        if (conditionDefinition.Guard?.Check(context) == false)
        {
            // Raise event
            return;
        }

        var holder = context.MatchContext.Match;
        var condition = GlobalCondition.Instantiate(conditionDefinition, stacksToApply);
        holder.ApplyCondition(condition);
        
        await conditionExecutor.Apply(condition, holder, context.MatchContext, collector, cancellationToken);
    }
}