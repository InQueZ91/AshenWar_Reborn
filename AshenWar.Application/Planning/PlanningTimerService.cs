using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Hubs;
using Application.ValueObjects;
using Domain.Entities.Match;
using Domain.ValueObjects.Identifiers.Match;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Application.Planning;

public sealed class PlanningTimerService(
    IMatchRepository matchRepository,
    IHubContext<GameHub> hubContext) : BackgroundService
{
    private CancellationTokenSource? _timerCts;
    private MatchId? _activeMatchId;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RecoverOnStartup(stoppingToken);
    }

    public async Task StartPlanningTimer(MatchId matchId, Turn turn, CancellationToken stoppingToken)
    {
        _timerCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        _activeMatchId = matchId;
        
        var remaining = turn.PlanningStartedAt + turn.PlanningDuration - DateTimeOffset.UtcNow;
        if (remaining <= TimeSpan.Zero)
        {
            await BroadcastExpired(matchId, turn);
            return;
        }

        try
        {
            await Task.Delay(remaining, _timerCts.Token);
            await BroadcastExpired(matchId, turn);
        }
        catch (TaskCanceledException)
        {
            // Both players submitted early => clean exit
        }
        finally
        {
            _timerCts = null;
            _activeMatchId = null;
        }
    }
    
    public void CancelTimer()
    {
        _timerCts?.Cancel();
    }

    private async Task RecoverOnStartup(CancellationToken stoppingToken)
    {
        var match = await matchRepository.GetActivePlanningMatchAsync(stoppingToken);
        if (match is null) return;

        var turn = match.CurrentTurn;
        if (turn.BothSubmitted)
            return;
        
        await StartPlanningTimer(match.Id, turn, stoppingToken);
    }

    private async Task BroadcastExpired(MatchId matchId, Turn turn)
    {
        await hubContext.Clients
            .Group(matchId.Value.ToString())
            .SendAsync("PlanningExpired", new PlanningExpired(turn.Id, matchId));
    }
}