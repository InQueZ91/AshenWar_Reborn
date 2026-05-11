using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using MediatR;

namespace Application.Auth.Handler;

public record LogoutRequest(string RefreshToken) : IRequest;

public sealed class LogoutHandler(
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService
    ) : IRequestHandler<LogoutRequest>
{
    public async Task Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var hash = tokenService.HashToken(request.RefreshToken);
        var stored = await refreshTokenRepository.GetByTokenHashAsync(hash, cancellationToken);
        
        // If token not found or already revoked - still return success
        // Never tell the client whether the token existed
        if (stored is null || stored.IsRevoked)
            return;
        
        stored.Revoke();
        await refreshTokenRepository.SaveAsync(stored, cancellationToken);
    }
}