using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using Microsoft.EntityFrameworkCore;

namespace AshenWar.Infrastructure.Persistence.Postgres;

public sealed class PostgresUserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default)
        => await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) 
        => await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) 
        => await dbContext.Users.AnyAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default) 
        => await dbContext.Users.AnyAsync(u => u.Username == username, ct);

    public async Task SaveAsync(User user, CancellationToken ct = default)
    {
        var exists = await dbContext.Users.AnyAsync(u => u.Id == user.Id, ct);
        if (exists)
            dbContext.Users.Update(user);
        else
            await dbContext.Users.AddAsync(user, ct);
        
        await dbContext.SaveChangesAsync(ct);
    }
}