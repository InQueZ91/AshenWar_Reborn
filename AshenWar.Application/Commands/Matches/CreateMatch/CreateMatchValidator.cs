using FluentValidation;

namespace Application.Commands.Matches.CreateMatch;

public sealed class CreateMatchValidator : AbstractValidator<CreateMatchRequest>
{
    public CreateMatchValidator()
    {
        RuleFor(x => x.MatchDefinitionId).NotNull();
        RuleFor(x => x.MapDefinitionId).NotNull();
        RuleFor(x => x.BlueUserId).NotNull();
        RuleFor(x => x.RedUserId).NotNull();
        RuleFor(x => x.BlueRoster).NotEmpty();
        RuleFor(x => x.RedRoster).NotEmpty();
        
        RuleFor(x => x.BlueUserId).NotEqual(x => x.RedUserId)
            .WithMessage("Player must be different.");
    }
}