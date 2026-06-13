using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Interfaces.Actions;

namespace AshenWar.Application.Contracts.Execution;

public interface IActionHandler<TDefinition> where TDefinition : IActionDefinition
{
    Task Execute(TDefinition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken);
}