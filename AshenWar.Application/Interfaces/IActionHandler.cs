using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Actions;
using Domain.Interfaces.Actions;

namespace Application.Interfaces;

public interface IActionHandler<TDefinition> where TDefinition : IActionDefinition
{
    Task Execute(TDefinition definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken);
}