using System.Linq;
using Domain.Entities.Actions;
using Domain.Interfaces.Abilities;

namespace Domain.Entities.Abilities.Validators;

public sealed class OrValidator(params IValidator[] validators) : IValidator
{
    public bool Check(ActionContext context) => validators.Any(v => v.Check(context));
}