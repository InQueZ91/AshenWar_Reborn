using System.Threading;
using System.Threading.Tasks;
using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Domain.Exceptions;
using MediatR;

namespace AshenWar.Application.Auth.Handler;

public record RefreshRequest(string RefreshToken) : IRequest<AuthResult>;

public sealed class RefreshHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    AuthService authService,
    ITokenService tokenService) : IRequestHandler<RefreshRequest, AuthResult>
{
    public async Task<AuthResult> Handle(RefreshRequest request, CancellationToken ct)
    {
        var hash = tokenService.HashToken(request.RefreshToken);
        var stored = await refreshTokenRepository.GetByTokenHashAsync(hash, ct);
        
        if (stored is null || !stored.IsValid())
            throw new DomainException("Invalid or expired refresh token");
        
        var user = await userRepository.GetByIdAsync(stored.UserId, ct);
        
        if (user is null)
            throw new DomainException("User not found");
        
        // Rotate - revoke old token, issue new one
        // If attacker steals and uses a refresh token,
        // the real user's next refresh will fail - detectable
        stored.Revoke();
        await refreshTokenRepository.SaveAsync(stored, ct);

        return await authService.IssueTokenAsync(user, ct);
    }
}