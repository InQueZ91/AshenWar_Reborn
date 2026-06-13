using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Application.Contracts.Auth;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task SaveAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAllForUserAsync(UserId userId, CancellationToken ct = default);
}