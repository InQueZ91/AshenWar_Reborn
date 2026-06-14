using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Definitions;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Domain.Entities.Abilities.Passives;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Ability;
using AshenWar.Domain.Interfaces.Abilities;

namespace AshenWar.Application.Execution.Handlers.Ability;

public sealed class GrantPassiveHandler(IPassiveDefinitionRepository passiveDefinitionRepository) : IActionHandler<GrantPassive>
{
    public async Task Execute(GrantPassive definition,
        ActionContext context,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        // Fetch definition
        var passiveAbilityDefinition = await passiveDefinitionRepository.FindPassiveAsync(
            definition.PassiveDefinitionId,
            cancellationToken);

        if (passiveAbilityDefinition is null)
        {
            return;
        }
        
        // Instantiate passive
        var passiveAbility = Passive.Instantiate(passiveAbilityDefinition);

        // Add passive to holder
        var owner = context.Source;
        if (owner is not IPassive holder)
        {
            return;
        }
        holder.AddPassive(passiveAbility);
        
        // Register passive to trigger registry
        var anchorPosition = context.Targets.FirstOrDefault()?.Position;
        context.MatchContext.TriggerRegistry.Register(context.Source, passiveAbility, anchorPosition);
    }
}