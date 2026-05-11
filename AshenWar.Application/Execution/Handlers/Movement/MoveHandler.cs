using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Movement;
using Domain.Interfaces.Entities;

namespace Application.Execution.Handlers.Movement;

public sealed class MoveHandler : IActionHandler<Move>
{
    public Task Execute(Move definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var targets = context.Targets.OfType<IUnitCommand>();
        foreach (var target in targets)
        {
            target.MoveTo(definition.Destination);
        }
        
        return Task.CompletedTask;
    }
}