using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts.Execution;
using AshenWar.Application.Contracts.Notifications;
using AshenWar.Application.Events;
using AshenWar.Application.Validators;
using AshenWar.Application.ValueObjects;
using AshenWar.Domain.Entities.Abilities;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Entities.Actions.Definitions.Ability.Cooldown;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.Interfaces.Entities;
using AshenWar.Domain.ValueObjects;
using AshenWar.Domain.ValueObjects.Orders;
using Microsoft.Extensions.Logging;

namespace AshenWar.Application.Execution;

public sealed class AbilityExecutor(
    ILogger<AbilityExecutor> logger,
    IGameNotifier gameNotifier,
    TriggerChain triggerChain,
    EffectExecutor effectExecutor,
    ActionExecutor actionExecutor)
{
    public async Task<ResolutionBatch> Execute(
        IReadOnlyUnit source,
        Ability ability,
        List<StepSelection> stepSelections,
        MatchContext matchContext,
        CancellationToken ct)
    {
        var failures = AffordabilityValidator.Validate(source, ability);
        if (failures.Count > 0)
        {
            // Broadcast affordability failure
            var formatted = failures.Select(FormatFailure).ToList();
            await gameNotifier.AbilityFailed(matchContext.Match.Id, source.Id, ability.Id, formatted, ct);
            
            return new ResolutionBatch(ImmutableList<ResolutionTick>.Empty);
        }

        var collector = new DomainEventCollector();
        
        // Execute Ability
        await SpendCost(source, ability, matchContext, collector, ct);
        await ExecuteSteps(ability.Definition, source, stepSelections, matchContext, collector, ct);
        
        return await triggerChain.Run(collector, matchContext, ct);
    }
    
    private async Task SpendCost(
        IReadOnlyUnit source,
        Ability ability,
        MatchContext matchContext,
        IDomainEventCollector collector,
        CancellationToken ct)
    {
        // Build self-context
        var selfContext = new ActionContext(source,
            new List<ITargetable>() { source },
            new HashSet<EntityTag>(),
            matchContext);

        // Spend costs
        foreach (var cost in ability.Definition.Costs)
            await actionExecutor.Execute(cost.SpendAction, selfContext, collector, ct);

        // Start cooldown
        var startCooldownAction = new StartAbilityCooldown(ability.Definition.Id);
        await actionExecutor.Execute(startCooldownAction, selfContext, collector, ct);
    }
    
    private async Task ExecuteSteps(
        AbilityDefinition definition,
        IReadOnlyUnit source, 
        IReadOnlyList<StepSelection> stepSelections,
        MatchContext matchContext,
        IDomainEventCollector collector,
        CancellationToken cancellationToken)
    {
        // Execute steps
        foreach (var step in definition.Steps)
        {
            // 1 Resolve targets
            var targets = new List<ITargetable>();

            // Get step selection
            var selection = stepSelections.FirstOrDefault(pi => pi.AbilityStepId == step.Id);
            if (selection is null)
            {
                logger.LogWarning(
                    "Missing step selection for step {StepId} on ability {AbilityId}",
                    step.Id,
                    definition.Id);
                continue;
            }
            
            if (source.Position is null)
            {
                logger.LogWarning(
                    "Cannot resolve targets for step {StepId} because source {SourceId} has no position",
                    step.Id,
                    source.Id);
                continue;
            }

            var selectedTarget = selection.Target.Resolve(matchContext.Board, source);
            var candidates = step.Shape.Resolve(source.Position, selectedTarget, matchContext.Board);
            targets.AddRange(candidates);

            // Filter targets
            var filtered = step.Filter?.Apply(targets, source) ?? targets;

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
            if (step.Validator?.Check(actionContext) == false)
                continue;

            // Execute Actions
            foreach (var action in step.Actions)
                await actionExecutor.Execute(action, actionContext, collector, cancellationToken);
            
            // Execute Effect
            foreach (var effectDefinitionId in step.Effects)
                await effectExecutor.Execute(effectDefinitionId, actionContext, collector, cancellationToken);
        }
    }
    
    private static string FormatFailure(AffordabilityFailure failure) => failure switch
    {
        OnCooldown f         => $"On cooldown — {f.RemainingTurns} turns remaining",
        CannotAffordCost f   => $"Not enough {f.Data.ResourceType} " +
                                $"— need {f.Data.Required}, have {f.Data.Available}",
        _ => throw new InvalidOperationException($"Unknown AffordabilityFailure type: {failure.GetType().Name}")
    };
}