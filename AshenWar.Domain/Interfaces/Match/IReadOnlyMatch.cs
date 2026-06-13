using AshenWar.Domain.Entities.Conditions.Global;
using AshenWar.Domain.Enums;
using AshenWar.Domain.Interfaces.Conditions;
using AshenWar.Domain.ValueObjects.Identifiers.Match;

namespace AshenWar.Domain.Interfaces.Match;

public interface IReadOnlyMatch : IHasConditions<GlobalCondition>
{
    MatchId Id { get; }
    MatchPhase Phase { get; }
    IReadOnlyBoard ReadOnlyBoardState { get; }
}