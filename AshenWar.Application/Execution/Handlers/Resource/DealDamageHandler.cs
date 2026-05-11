using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Resource;
using Domain.Entities.Modifiers;
using Domain.Entities.Stats;
using Domain.Events;
using Domain.Interfaces.Entities;

namespace Application.Execution.Handlers.Resource;

public sealed class DealDamageHandler : IActionHandler<DealDamage> 
{
    public Task Execute(DealDamage definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken = default)
    {
        if (context.Source is not IUnitCommand caster) return Task.CompletedTask;
        
        // Every target that is unit gets damaged
        var targetUnits = context.Targets.OfType<IUnitCommand>();
        var abilityTags = context.AbilityTags;
        foreach (var target in targetUnits)
        {
            // Step 1 - Offensive calculation
            var baseValue = definition.Amount.Resolve(context);
            var offensiveDamage = ModifierCalculator.Calculate(
                baseValue,
                caster.Conditions,
                StatDefinition.BaseDamage.Id,
                abilityTags
            );
            
            // Step 2 - Defensive calculation
            var finalDamage = ModifierCalculator.Calculate(
                offensiveDamage,
                target.Conditions,
                StatDefinition.IncomingDamage.Id,
                abilityTags
            );
            
            collector.Collect(new DamageDealt(target.Id, finalDamage, abilityTags));
            
            target.TakeDamage(finalDamage);
        }
        
        return Task.CompletedTask;
    }
}