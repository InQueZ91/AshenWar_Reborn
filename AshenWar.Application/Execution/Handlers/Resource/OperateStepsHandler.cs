using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Resource;
using Domain.Interfaces.Entities;

namespace Application.Execution.Handlers.Resource;

public sealed class OperateStepsHandler : IActionHandler<OperateSteps>
{
    public Task Execute(OperateSteps definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        var targets = context.Targets.OfType<IUnitCommand>();
        foreach (var target in targets)
        {
            var delta = (int)definition.Amount.Resolve(context);
            target.OperateSteps(delta);
        }
        
        return Task.CompletedTask;
    }
}