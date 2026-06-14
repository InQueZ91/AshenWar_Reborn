using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Costs;
using AshenWar.Domain.Enums.Conditions;
using AshenWar.Domain.Events;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Costs;

public sealed class SpendOverloadStacksHandler : IActionHandler<SpendOverloadStacks>
{
    public Task Execute(SpendOverloadStacks definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        if (context.Source is not IUnit unit) 
            return Task.CompletedTask;

        var overloadCondition = unit.GetConditionWithTag(ConditionTag.Overload);
        if (overloadCondition is null) 
            return Task.CompletedTask;
        
        unit.RemoveCondition(overloadCondition.Id);
        
        collector.Collect(new AbilityCostPaid(unit.Id, "Overload", definition.Stacks));
        
        return Task.CompletedTask;
    }
}