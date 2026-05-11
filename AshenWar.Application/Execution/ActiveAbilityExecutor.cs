using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Events;
using Application.Hubs;
using Application.Interfaces;
using Application.Validators;
using Application.ValueObjects;
using Domain.Entities.Abilities.Active;
using Domain.Entities.Actions;
using Domain.Entities.Actions.Definitions.Ability;
using Domain.Entities.Match;
using Domain.Interfaces.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.SignalR;

namespace Application.Execution;

public sealed class ActiveAbilityExecutor(
    IHubContext<GameHub> hubContext,
    TriggerChain triggerChain,
    EffectExecutor effectExecutor,
    ActionExecutor actionExecutor)
{
    public async Task<ResolutionBatch> Execute(
        IUnit source,
        ActiveAbility activeAbility,
        List<PhaseInput> phaseInputs,
        MatchContext matchContext,
        CancellationToken ct)
    {
        var failures = AffordabilityValidator.Validate(source, activeAbility);
        if (failures.Count > 0)
        {
            // Broadcast affordability failure
            var formatted = failures.Select(FormatFailure).ToList();
            await hubContext.Clients.Group(matchContext.Match.Id.ToString())
                .SendAsync("AbilityFailed", new
                {
                    UnitId = source.Id,
                    AbilityId = activeAbility.Id,
                    Failures = formatted
                }, ct);
            
            return new ResolutionBatch(ImmutableList<ResolutionTick>.Empty);
        }

        var collector = new DomainEventCollector();
        
        // Execute Ability
        await SpendCost(source, activeAbility, matchContext, collector, ct);
        await ExecutePhases(activeAbility.Definition, source, phaseInputs, matchContext, collector, ct);
        
        return await triggerChain.Run(collector, matchContext, ct);
    }
    
    private async Task SpendCost(
        IUnit source,
        ActiveAbility activeAbility,
        MatchContext matchContext,
        IDomainEventCollector collector,
        CancellationToken ct)
    {
        // Build self-context
        var selfContext = new ActionContext(source,
            new List<ITargetable>(),
            new HashSet<EntityTag>(),
            matchContext);

        // Spend costs
        foreach (var cost in activeAbility.Definition.Costs)
            await actionExecutor.Execute(cost.SpendAction, selfContext, collector, ct);

        // Start cooldown
        var startCooldownAction = new StartAbilityCooldown{ AbilityDefinitionId = activeAbility.Definition.Id};
        await actionExecutor.Execute(startCooldownAction, selfContext, collector, ct);
    }
    
    private async Task ExecutePhases(
        ActiveAbilityDefinition definition,
        IUnit source, List<PhaseInput> phaseInputs,
        MatchContext matchContext,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        // Execute phases
        foreach (var phase in definition.Phases)
        {
            // 1 Resolve targets
            var targets = new List<ITargetable>();
            
            // Get phase input
            var phaseInput = phaseInputs.FirstOrDefault(pi => pi.AbilityPhaseId == phase.Id);
            if (phaseInput is null) // candidates is the source
            {
                targets.Add(source);
            }
            else // candidates resolved from shape
            {
                // NOTE: null shape means self
                var candidates = phase.Shape?.Resolve(source.Position, phaseInput.SelectedTarget, matchContext.Board);
                targets.AddRange(candidates ?? new List<ITargetable>(){ source });
            }
            
            // Filter targets
            var filtered = phase.Filter?.Apply(targets, source) ?? targets;
            
            // 2 Execute effects
            // Build action context
            var actionContext = new ActionContext(
                source,
                filtered.ToList(),
                definition.Tags,
                matchContext
            );

            // Validate phase
            // Skip phase if validator fails
            if (phase.Validator?.Check(actionContext) == false)
            {
                // Might want to log this
                continue;
            }

            // Execute Effect
            foreach (var effectDefinitionId in phase.Effects) 
                await effectExecutor.Execute(effectDefinitionId, actionContext, collector, cancellationToken);
        }

    }
    
    private static string FormatFailure(AffordabilityFailure failure) => failure switch
    {
        OnCooldown f         => $"On cooldown — {f.RemainingTurns} turns remaining",
        CannotAffordCost f   => $"Not enough {f.Data.ResourceType} " +
                                $"— need {f.Data.Required}, have {f.Data.Available}",
        _                    => "Cannot use ability"
    };
}