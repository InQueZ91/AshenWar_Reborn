using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Costs;
using Domain.Enums.Conditions;
using Domain.Events;
using Domain.Interfaces.Entities;

namespace Application.Execution.Handlers.Costs;

public sealed class SpendOverloadStacksHandler : IActionHandler<SpendOverloadStacks>
{
    public Task Execute(SpendOverloadStacks definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        if (context.Source is not IUnitCommand unit) 
            return Task.CompletedTask;

        var overloadCondition = unit.GetConditionWithTag(ConditionTag.Overload);
        if (overloadCondition is null) 
            return Task.CompletedTask;
        
        unit.RemoveCondition(overloadCondition.Id);
        
        collector.Collect(new AbilityCostPaid(unit.Id, "Overload", definition.Stacks));
        
        return Task.CompletedTask;
    }
}