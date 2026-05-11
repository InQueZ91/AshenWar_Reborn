using Domain.Entities.Conditions.Global;
using Domain.Enums;
using Domain.Interfaces.Conditions;
using Domain.ValueObjects.Identifiers.Match;

namespace Domain.Interfaces.Match;

public interface IMatch : IConditionHolder<GlobalCondition>
{
    MatchId Id { get; }
    MatchPhase Phase { get; }
    IBoard BoardState { get; }
}