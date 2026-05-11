using Domain.Interfaces;
using Domain.Interfaces.Match;

namespace Domain.Entities.Match;

public sealed class MatchContext(IMatchCommand match, ITriggerRegistry triggerRegistry, int turnNumber)
{
    public IMatchCommand Match { get; } = match;
    public IBoardCommand Board => Match.Board;
    public ITriggerRegistry TriggerRegistry { get; } = triggerRegistry;
    public int TurnNumber { get; } = turnNumber;
}