using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Resource;
using AshenWar.Domain.Entities.Modifiers;
using AshenWar.Domain.Entities.Stats;
using AshenWar.Domain.Events;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Application.Execution.Handlers.Resource;

public sealed class DealDamageHandler : IActionHandler<DealDamage> 
{
    public Task Execute(DealDamage definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        if (context.Source is not IUnit caster) return Task.CompletedTask;
        
        // Every target that is unit gets damaged
        var targetUnits = context.Targets.OfType<IUnit>();
        var abilityTags = context.AbilityTags;
        
        foreach (var target in targetUnits)
        {
            // Step 1 - Offensive calculation
            var baseValue = definition.Amount.Resolve(context);
            var offensiveDamage = ModifierCalculator.Calculate(
                baseValue,
                StatDefinition.Damage,
                caster.Conditions,
                abilityTags
            );
            
            // Step 2 - Defensive calculation
            var finalDamage = ModifierCalculator.Calculate(
                offensiveDamage,
                StatDefinition.IncomingDamage,
                target.Conditions,
                abilityTags
            );
            
            collector.Collect(new DamageDealt(target.Id, finalDamage, abilityTags));
            
            target.TakeDamage(finalDamage);
        }
        
        return Task.CompletedTask;
    }
}