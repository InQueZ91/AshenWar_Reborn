using System.Linq;
using FluentValidation;

namespace Application.Commands.Matches.ConfirmDeploy;

public sealed class ConfirmDeploymentValidator : AbstractValidator<ConfirmDeploymentRequest>
{
    public ConfirmDeploymentValidator()
    {
        RuleFor(x => x.MatchId).NotNull();
        RuleFor(x => x.UserId).NotNull();
        RuleFor(x => x.Slots).NotEmpty();
        
        RuleForEach(x => x.Slots).ChildRules(slot =>
        {
            slot.RuleFor(s => s.UnitDefinitionId).NotNull();
            slot.RuleFor(s => s.Position).NotNull();
        });
        
        RuleFor(x => x.Slots)
            .Must(slots => slots.Select(s => s.Position).Distinct().Count() == slots.Count)
            .WithMessage("Duplicate Position in slots.");
    }
}