using System;
using AshenWar.Domain.Entities.Users;

namespace AshenWar.Application.Contracts.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    (string RawToken, string TokenHash) GenerateRefreshToken();
    
    string HashToken(string raw);
    
    DateTimeOffset GetRefreshTokenExpiry();
}