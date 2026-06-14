using AshenWar.Domain.Interfaces;
using AshenWar.Domain.Interfaces.Abilities;
using AshenWar.Domain.Interfaces.Match;

namespace AshenWar.Domain.Entities.Match;

public sealed class MatchContext(IMatch match, ITriggerRegistry triggerRegistry, int turnNumber)
{
    public IMatch Match { get; } = match;
    public IBoard Board => Match.Board;
    public ITriggerRegistry TriggerRegistry { get; } = triggerRegistry;
    public int TurnNumber { get; } = turnNumber;
}