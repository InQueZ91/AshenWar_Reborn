using System;
using System.Collections.Generic;
using Domain.ValueObjects.Identifiers.Players;
using Domain.ValueObjects.Identifiers.Units;

namespace Domain.Entities.Users;

public sealed class User
{
    private readonly List<UnitDefinitionId> _units = [];

    public UserId Id { get; private set; }
    public string Username { get; private set;}
    public string Email { get; private set; }
    public string? PasswordHash {get; private set;} // null = OAuth user (future)
    public string? GoogleId {get; private set;} // null = not linked (future)
    public string Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    public IReadOnlyList<UnitDefinitionId> Units => _units.AsReadOnly();
    
    // Constructor
    private User() { } // EF Core
    
    public static User Create(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        return new User
        {
            Id = UserId.New(),
            Username = username,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = UserRoles.Player,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
    
    // Unit collection
    public void AddUnit(UnitDefinitionId unitDefinitionId)
    {
        ArgumentNullException.ThrowIfNull(unitDefinitionId);
        if (_units.Contains(unitDefinitionId))
            return; // idempotent - earning same unit twice shouldn't duplicate
        _units.Add(unitDefinitionId);
    }
    public void RemoveUnit(UnitDefinitionId unitDefinitionId)
    {
        ArgumentNullException.ThrowIfNull(unitDefinitionId);
        _units.Remove(unitDefinitionId);
    }
    
    // Profile
    public void ChangeUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ArgumentException("Username cannot be empty.", nameof(newUsername));
        Username = newUsername;
    }
    
    // Auth helpers
    public bool HasPassword() => PasswordHash is not null;
    public bool OwnsUnit(UnitDefinitionId unitDefinitionId) => _units.Contains(unitDefinitionId);
    
    public void PromoteToAdmin() => Role = UserRoles.Admin;
}