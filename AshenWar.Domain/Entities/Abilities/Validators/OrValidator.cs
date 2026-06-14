using System.Collections.Generic;
using System.Linq;
using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Interfaces.Abilities;

namespace AshenWar.Domain.Entities.Abilities.Validators;

public sealed record OrValidator : IValidator
{
    public required IReadOnlyList<IValidator> Validators { get; init; }
    public bool Check(ActionContext context) => Validators.Any(v => v.Check(context));
}