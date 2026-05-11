using Domain.Entities.Actions;
using Domain.Interfaces.Abilities;

namespace Domain.Entities.Abilities.Validators.Primitives;

public sealed class TurnNumberModValidator(int modulo, int remainder) : IValidator
{
    public bool Check(ActionContext context) 
        => context.MatchContext.TurnNumber % modulo == remainder;
}