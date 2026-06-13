using System.Threading;
using System.Threading.Tasks;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.ValueObjects.Identifiers.Players;

namespace AshenWar.Application.Contracts.Auth;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
    Task SaveAsync(User user, CancellationToken ct = default);
}