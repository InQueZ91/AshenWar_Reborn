using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Resource;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Resource;

public sealed class OperateStaminaHandler : IActionHandler<OperateStamina>
{
    public Task Execute(OperateStamina definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        var targets = context.Targets.OfType<IUnit>();
        foreach (var target in targets)
        {
            var delta = (int)definition.Amount.Resolve(context);
            target.OperateStamina(delta);
            // No semantic event here - mechanical event sufficient for effect pipeline
        }
        
        return Task.CompletedTask;
    }
}