using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Ability;
using Domain.Interfaces.Abilities;

namespace Application.Execution.Handlers.Ability;

public sealed class StartAbilityCooldownHandler : IActionHandler<StartAbilityCooldown>
{
    public Task Execute(StartAbilityCooldown definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        foreach (var target in context.Targets)
        {
            if (target is not IAbilityHolder abilityHolder) continue;
            
            var activeAbility = abilityHolder.GetActiveAbilityByDefinitionId(definition.AbilityDefinitionId);
            activeAbility?.StartCooldown();
        }

        return Task.CompletedTask;
    }
}