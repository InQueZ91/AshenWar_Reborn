using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Costs;
using Domain.Events;
using Domain.Interfaces.Entities;

namespace Application.Execution.Handlers.Costs;

public sealed class SpendStepsHandler : IActionHandler<SpendSteps>
{
    public Task Execute(SpendSteps definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        if (context.Source is not IUnitCommand unit) return Task.CompletedTask;
        
        unit.OperateSteps(-1 * definition.Amount); // negative = spend
        
        collector.Collect(new AbilityCostPaid(unit.Id, "Steps", definition.Amount));
        
        return Task.CompletedTask;
    }
}