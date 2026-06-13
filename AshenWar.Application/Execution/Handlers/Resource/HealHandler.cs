using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Resource;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Resource;

public sealed class HealHandler : IActionHandler<Heal>
{
    public Task Execute(Heal definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        var targetUnits = context.Targets.OfType<IUnit>();
        foreach (var target in targetUnits)
        {
            var amount = (int)definition.Amount.Resolve(context);
            target.RestoreHealth(amount);
        }
        
        return Task.CompletedTask;
    }
}