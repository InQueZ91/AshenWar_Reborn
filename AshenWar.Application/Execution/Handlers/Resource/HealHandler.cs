using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Resource;
using Domain.Interfaces.Entities;

namespace Application.Execution.Handlers.Resource;

public sealed class HealHandler : IActionHandler<Heal>
{
    public Task Execute(Heal definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        var targetUnits = context.Targets.OfType<IUnitCommand>();
        foreach (var target in targetUnits)
        {
            var amount = (int)definition.Amount.Resolve(context);
            target.RestoreHealth(amount);
        }
        
        return Task.CompletedTask;
    }
}