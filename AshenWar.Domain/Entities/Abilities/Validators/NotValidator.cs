using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Interfaces.Abilities;

namespace AshenWar.Domain.Entities.Abilities.Validators;

public sealed record NotValidator : IValidator
{
    public required IValidator Validator { get; init; }
    public bool Check(ActionContext context) => !Validator.Check(context); 
}