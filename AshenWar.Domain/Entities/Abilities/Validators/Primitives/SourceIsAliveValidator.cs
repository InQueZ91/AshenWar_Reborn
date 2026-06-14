using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Entities;

namespace AshenWar.Domain.Entities.Abilities.Validators.Primitives;

public sealed record SourceIsAliveValidator : IValidator
{
    public bool Check(ActionContext context)
        => context.Source is IReadOnlyUnit { IsAlive: true };
}