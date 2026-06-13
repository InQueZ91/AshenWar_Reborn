using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Movement;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Movement;

public sealed class MoveHandler : IActionHandler<Move>
{
    public Task Execute(Move definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var targets = context.Targets.OfType<IUnit>();
        foreach (var target in targets)
        {
            target.MoveTo(definition.Destination);
        }
        
        return Task.CompletedTask;
    }
}