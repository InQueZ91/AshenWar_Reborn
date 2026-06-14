using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Domain.Entities.Users;

namespace AshenWar.Application.Auth;

public sealed class AuthService(
    IRefreshTokenRepository refreshTokenRepositoryRepository,
    ITokenService tokenService)
{
    public async Task<AuthResult> IssueTokenAsync(User user, CancellationToken ct)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var (rawRefresh, refreshHash) = tokenService.GenerateRefreshToken();

        // TokenService knows the expiry - it has JwtSettings injected
        var expiry = tokenService.GetRefreshTokenExpiry();
        
        var refreshToken = RefreshToken.Create(user.Id, refreshHash, expiry);
        await refreshTokenRepositoryRepository.SaveAsync(refreshToken, ct);

        return new AuthResult(accessToken, rawRefresh);
    }
}