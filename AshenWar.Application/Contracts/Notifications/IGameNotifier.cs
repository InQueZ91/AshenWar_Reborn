using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Events;
using AshenWar.Application.ValueObjects.Roll;
using AshenWar.Domain.Entities.Lobbies;
using AshenWar.Domain.Entities.Match;
using AshenWar.Domain.ValueObjects.Identifiers;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using AshenWar.Domain.ValueObjects.LocalIdentifiers.Abilities;

namespace AshenWar.Application.Contracts.Notifications;

public interface IGameNotifier
{
    Task LobbyFounded(Lobby lobby, CancellationToken ct);
    Task LobbyDeleted(Lobby lobby, CancellationToken ct);
    Task LobbyUpdated(Lobby lobby, CancellationToken ct);
    
    Task MatchStarted(MatchId matchId, Match match, CancellationToken ct);
    Task TurnInitiativeRolled(MatchId matchId, TurnInitiative initiative, CancellationToken ct);
    Task PlanningStarted(MatchId matchId, Turn nextTurn, CancellationToken ct);
    Task PlanningExpired(MatchId matchId, CancellationToken ct);
    Task ResolutionCompleted(MatchId matchId, TurnResolutionResult result, CancellationToken ct);
    Task MatchEnded(MatchId matchId, UserId? winnerId, CancellationToken ct);

    Task AbilityFailed(MatchId matchId, UnitId sourceId, AbilityId abilityId, List<string> failures, CancellationToken ct);
}