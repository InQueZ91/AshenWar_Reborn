using System;
using Domain.ValueObjects.Identifiers.Players;

namespace Domain.Entities.Users;

public sealed class RefreshToken
{
    public Guid Id { get; private set;}
    public UserId UserId { get; private set;}
    public string TokenHash { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set;}
    public DateTimeOffset CreatedAt { get; private set;}
    public bool IsRevoked { get; private set;}
    
    private RefreshToken() { } // EF Core

    public static RefreshToken Create(UserId userId, string tokenHash, DateTimeOffset expiresAt)
    {
        ArgumentNullException.ThrowIfNull(userId);
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash cannot be empty.", nameof(tokenHash));

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            IsRevoked = false
        };
    }
    
    public void Revoke() => IsRevoked = true;
    
    public bool IsValid() => !IsRevoked && ExpiresAt > DateTimeOffset.UtcNow;
}