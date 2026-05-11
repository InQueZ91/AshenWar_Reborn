using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Interfaces;
using Domain.Entities.Abilities.Passive;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Ability;
using Domain.Interfaces.Abilities.Passive;

namespace Application.Execution.Handlers.Ability;

public sealed class GrantPassiveHandler(IAbilityRepository abilityRepository) : IActionHandler<GrantPassive>
{
    public async Task Execute(GrantPassive definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        // Fetch definition
        var passiveAbilityDefinition = await abilityRepository.GetPassiveAbilityAsync(
            definition.PassiveAbilityDefinitionId,
            cancellationToken);
        
        // Instantiate passive
        var passiveAbility = PassiveAbility.Instantiate(passiveAbilityDefinition);

        // Add passive to holder
        var owner = context.Source;
        if (owner is not IPassiveAbilityCommand holder)
        {
            return;
        }
        holder.AddPassive(passiveAbility);
        
        // Register passive to trigger registry
        var anchorPosition = context.Targets.FirstOrDefault()?.Position;
        context.MatchContext.TriggerRegistry.Register(context.Source, passiveAbility, anchorPosition);
    }
}