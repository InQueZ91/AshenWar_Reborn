using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Domain.Entities.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AshenWar.Infrastructure.JWT;

public sealed class TokenService(IOptions<JwtSettings> settings) : ITokenService
{
    private readonly JwtSettings _settings = settings.Value;
    private readonly JsonWebTokenHandler _handler = new();

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("username", user.Username),
                    new Claim("role", user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ]
            ),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenMinutes).UtcDateTime,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return _handler.CreateToken(descriptor);
    }

    public (string RawToken, string TokenHash) GenerateRefreshToken()
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hash = HashToken(raw);
        return (raw, hash);
    }

    public string HashToken(string raw)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }

    public DateTimeOffset GetRefreshTokenExpiry()
    {
        return DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenDays);
    }
}