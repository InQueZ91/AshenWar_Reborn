using Domain.Entities.Actions;
using Domain.Interfaces.Abilities;

namespace Domain.Entities.Abilities.Validators;

public sealed class NotValidator(IValidator validator) : IValidator
{
    public bool Check(ActionContext context) => !validator.Check(context); 
}