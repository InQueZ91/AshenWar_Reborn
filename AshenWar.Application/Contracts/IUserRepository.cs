using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.ValueObjects.Identifiers.Players;

namespace Application.Contracts;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
    Task SaveAsync(User user, CancellationToken ct = default);
}