using System;
using System.Threading.Tasks;
using Domain.ValueObjects.Identifiers.Match;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs;

public sealed class GameHub : Hub
{
    public async Task JoinMatch(MatchId matchId) 
        => await Groups.AddToGroupAsync(Context.ConnectionId, matchId.ToString());
    
    public async Task LeaveMatch(MatchId matchId) 
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId.ToString());

    public override async Task OnDisconnectedAsync(Exception? exception) 
        => await base.OnDisconnectedAsync(exception);
}