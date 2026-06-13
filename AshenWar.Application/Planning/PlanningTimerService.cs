using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Notifications;
using AshenWar.Domain.ValueObjects.Identifiers.Match;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AshenWar.Application.Planning;

public sealed class PlanningTimerService(
    ChannelReader<TimerMessage> reader,
    IServiceScopeFactory scopeFactory,
    IGameNotifier gameNotifier,
    ILogger<PlanningTimerService> logger) : BackgroundService
{
    private readonly ConcurrentDictionary<MatchId, CancellationTokenSource> _timers = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RecoverOnStartup(stoppingToken);

        await foreach (var message in reader.ReadAllAsync(stoppingToken))
        {
            switch (message)
            {
                case StartTimer msg:
                    StartTimer(msg.MatchId, msg.StartedAt, msg.Duration, stoppingToken);
                    break;
                case CancelTimer msg:
                    CancelTimer(msg.MatchId);
                    break;
            }
        }
    }

    private void StartTimer(MatchId matchId, DateTimeOffset startedAt, TimeSpan duration,
        CancellationToken stoppingToken)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        if (!_timers.TryAdd(matchId, cts))
        {
            cts.Dispose();
            logger.LogWarning("Timer already running for match {MatchId}. Ignoring.", matchId);
            return;
        }

        _ = RunTimerAsync(matchId, startedAt, duration, cts);
    }

    private void CancelTimer(MatchId matchId)
    {
        if (_timers.TryRemove(matchId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    private async Task RunTimerAsync(MatchId matchId, DateTimeOffset startedAt, TimeSpan duration,
        CancellationTokenSource cts)
    {
        try
        {
            var remaining = startedAt + duration - DateTimeOffset.UtcNow;
            if (remaining > TimeSpan.Zero)
                await Task.Delay(remaining, cts.Token);

            if (!cts.IsCancellationRequested)
                await gameNotifier.PlanningExpired(matchId, cts.Token);
        }
        catch (TaskCanceledException)
        {
            // Both players submitted early => clean exit
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Planning timer faulted for match {MatchId}", matchId);
        }
        finally
        {
            _timers.TryRemove(matchId, out _);
            cts.Dispose();
        }
    }
    
    private async Task RecoverOnStartup(CancellationToken stoppingToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        
        var matchRepository = scope.ServiceProvider.GetRequiredService<IMatchRepository>();
        
        var matches = await matchRepository.GetActivePlanningMatchesAsync(stoppingToken);
        
        foreach (var match in (matches ?? []).Where(match => !match.CurrentTurn.BothSubmitted))
        {
            var turn = match.CurrentTurn;
            StartTimer(match.Id, turn.PlanningStartedAt, turn.PlanningDuration, stoppingToken);
        }
    }
}