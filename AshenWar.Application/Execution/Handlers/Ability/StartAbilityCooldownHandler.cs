using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Ability.Cooldown;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Ability;

public sealed class StartAbilityCooldownHandler : IActionHandler<StartAbilityCooldown>
{
    public Task Execute(StartAbilityCooldown definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        foreach (var target in context.Targets)
        {
            if (target is not IUnit unit) continue;
            
            var ability = unit.GetAbilityByDefinitionId(definition.AbilityDefinitionId);
            if (ability is null) continue;

            var baseCooldown = ability.Definition.Stats.Get(StatDefinition.Cooldown);
            var modified = ModifierCalculator.Calculate(
                baseCooldown,
                StatDefinition.Cooldown,
                unit.Conditions,
                context.AbilityTags);
            
            ability.SetCooldown(modified);
        }

        return Task.CompletedTask;
    }
}