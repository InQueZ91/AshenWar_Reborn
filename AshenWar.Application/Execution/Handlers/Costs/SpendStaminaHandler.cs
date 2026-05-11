using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Costs;
using Domain.Events;
using Domain.Interfaces.Entities;

namespace Application.Execution.Handlers.Costs;

public sealed class SpendStaminaHandler : IActionHandler<SpendStamina>
{
    public Task Execute(SpendStamina definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        if (context.Source is not IUnitCommand unit) return Task.CompletedTask;
        
        unit.OperateStamina(-1 * definition.Amount); // negative = spend
        collector.Collect(new AbilityCostPaid(unit.Id, "Stamina", definition.Amount));

        return Task.CompletedTask;
    }
}