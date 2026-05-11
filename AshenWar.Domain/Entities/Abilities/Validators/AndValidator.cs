using System.Linq;
using Domain.Entities.Actions;
using Domain.Interfaces.Abilities;

namespace Domain.Entities.Abilities.Validators;

public sealed class AndValidator(params IValidator[] validators) : IValidator
{
    public bool Check(ActionContext context) => validators.All(v => v.Check(context));
}