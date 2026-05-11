using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.ValueObjects;
using Domain.Entities.Users;
using Microsoft.Extensions.Options;

namespace Application.Auth;

public sealed class AuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _settings;
    
    public AuthService(
        IRefreshTokenRepository refreshTokenRepositoryRepository,
        TokenService tokenService,
        IOptions<JwtSettings> settings)
    {
        _refreshTokenRepository = refreshTokenRepositoryRepository;
        _tokenService = tokenService;
        _settings = settings.Value;
    }
    
    public async Task<AuthResult> IssueTokenAsync(User user, CancellationToken ct)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var (rawRefresh, refreshHash) = _tokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshHash,
            DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenDays));

        await _refreshTokenRepository.SaveAsync(refreshToken, ct);

        return new AuthResult(accessToken, rawRefresh);
    }
}