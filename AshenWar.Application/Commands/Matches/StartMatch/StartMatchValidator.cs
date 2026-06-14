using System.Linq;
using FluentValidation;

namespace AshenWar.Application.Commands.Matches.StartMatch;

public sealed class StartMatchValidator : AbstractValidator<StartMatchRequest>
{
    public StartMatchValidator()
    {
        RuleFor(x => x.MatchDefinitionId).NotNull();
        RuleFor(x => x.MapDefinitionId).NotNull();
        RuleFor(x => x.BlueUserId).NotNull();
        RuleFor(x => x.RedUserId).NotNull();
        
        // Blue Deployment Plan
        RuleFor(x => x.BlueDeploymentPlan).NotEmpty();
        RuleForEach(x => x.BlueDeploymentPlan).ChildRules(slot =>
        {
            slot.RuleFor(s => s.UnitDefinitionId).NotNull();
            slot.RuleFor(s => s.Position).NotNull();
        });
        RuleFor(x => x.BlueDeploymentPlan)
            .Must(plans => plans.Select(s => s.Position).Distinct().Count() == plans.Count)
            .WithMessage("Duplicate Position in blue deployment plan.");

        // Red Deployment Plan
        RuleFor(x => x.RedDeploymentPlan).NotEmpty();
        RuleForEach(x => x.RedDeploymentPlan).ChildRules(slot =>
        {
            slot.RuleFor(s => s.UnitDefinitionId).NotNull();
            slot.RuleFor(s => s.Position).NotNull();
        });
        RuleFor(x => x.RedDeploymentPlan)
            .Must(plans => plans.Select(s => s.Position).Distinct().Count() == plans.Count)
            .WithMessage("Duplicate Position in red deployment plan.");       
        
        // Player
        RuleFor(x => x.BlueUserId).NotEqual(x => x.RedUserId)
            .WithMessage("Player must be different.");
    }
}