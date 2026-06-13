using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.ValueObjects.Identifiers.Effects;

namespace AshenWar.Application.Execution;

public class EffectExecutor(IEffectDefinitionRepository effectDefinitionRepository, ActionExecutor actionExecutor)
{
    public async Task Execute(EffectDefinitionId effectId,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        var effectDefinition = await effectDefinitionRepository.GetEffectAsync(effectId, cancellationToken);

        foreach (var action in effectDefinition.Actions)
            await actionExecutor.Execute(action, context, collector, cancellationToken);
    }
}