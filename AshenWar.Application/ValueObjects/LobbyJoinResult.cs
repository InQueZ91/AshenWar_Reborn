using AshenWar.Domain.ValueObjects.Identifiers;

namespace AshenWar.Application.ValueObjects;

public sealed record LobbyJoinResult
{
    public bool IsFound { get; private init; }
    public bool IsWaiting { get; private init; }
    public bool IsAlreadyInLobby { get; private init; }
    public LobbyId? LobbyId { get; private init; }

    public static LobbyJoinResult Found(LobbyId id)
        => new() { IsFound = true, LobbyId = id };

    public static LobbyJoinResult Waiting()
        => new() { IsWaiting = true };

    public static LobbyJoinResult AlreadyInLobby()
        => new() { IsAlreadyInLobby = true };
}