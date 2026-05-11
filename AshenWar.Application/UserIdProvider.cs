using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Application;

public sealed class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}