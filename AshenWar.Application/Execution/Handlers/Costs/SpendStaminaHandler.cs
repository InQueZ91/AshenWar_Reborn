using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Costs;
using AshenWar.Domain.Events;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Costs;

public sealed class SpendStaminaHandler : IActionHandler<SpendStamina>
{
    public Task Execute(SpendStamina definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        if (context.Source is not IUnit unit) return Task.CompletedTask;
        
        unit.OperateStamina(-1 * definition.Amount); // negative = spend
        collector.Collect(new AbilityCostPaid(unit.Id, "Stamina", definition.Amount));

        return Task.CompletedTask;
    }
}