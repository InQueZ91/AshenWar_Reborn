using Domain.Entities.Actions;
using Domain.Interfaces.Abilities;
using Domain.Interfaces.Entities;

namespace Domain.Entities.Abilities.Validators.Primitives;

public sealed class SourceIsAliveValidator : IValidator
{
    public bool Check(ActionContext context)
        => context.Source is IUnit { IsAlive: true };
}