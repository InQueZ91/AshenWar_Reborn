using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Costs;
using AshenWar.Domain.Events;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Costs;

public sealed class SpendStepsHandler : IActionHandler<SpendSteps>
{
    public Task Execute(SpendSteps definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        if (context.Source is not IUnit unit) return Task.CompletedTask;
        
        unit.OperateSteps(-1 * definition.Amount); // negative = spend
        
        collector.Collect(new AbilityCostPaid(unit.Id, "Steps", definition.Amount));
        
        return Task.CompletedTask;
    }
}