using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.ValueObjects.Identifiers.Players;

namespace Application.Contracts;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task SaveAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAllForUserAsync(UserId userId, CancellationToken ct = default);
}