using AshenWar.Domain.Entities.Actions;
using AshenWar.Domain.Interfaces.Abilities;

namespace AshenWar.Domain.Entities.Abilities.Validators.Primitives;

public sealed record TurnNumberModValidator(int Modulo, int Remainder) : IValidator
{
    public bool Check(ActionContext context) 
        => context.MatchContext.TurnNumber % Modulo == Remainder;
}