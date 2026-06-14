using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Condition;
using AshenWar.Domain.Entities.Conditions.Global;

namespace AshenWar.Application.Execution.Handlers.Condition;

public sealed class ApplyGlobalConditionHandler(
    IConditionDefinitionRepository conditionDefinitionRepository,
    ConditionExecutor<GlobalCondition> conditionExecutor)
    : IActionHandler<ApplyGlobalCondition>
{
    public async Task Execute(ApplyGlobalCondition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var conditionDefinition = await conditionDefinitionRepository.GetGlobalConditionAsync(
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