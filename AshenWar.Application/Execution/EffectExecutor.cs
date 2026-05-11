using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.ValueObjects.Identifiers.Effects;

namespace Application.Execution;

public class EffectExecutor(IEffectRepository effectRepository, ActionExecutor actionExecutor)
{
    public async Task Execute(EffectDefinitionId effectId,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var effectDefinition = await effectRepository.GetEffectAsync(effectId, cancellationToken);

        foreach (var action in effectDefinition.Actions)
            await actionExecutor.Execute(action, context, collector, cancellationToken);
    }
}