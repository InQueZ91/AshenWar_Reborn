using AshenWar.Application.Contracts;
using AshenWar.Application.Contracts.Auth;
using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using Microsoft.EntityFrameworkCore;

namespace AshenWar.Infrastructure.Persistence.Postgres;

public sealed class PostgresRefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default) 
        => await dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task SaveAsync(RefreshToken token, CancellationToken ct = default)
    {
        var exists = await dbContext.RefreshTokens.AnyAsync(t => t.Id == token.Id, ct);
        if (exists)
            dbContext.RefreshTokens.Update(token);
        else
            await dbContext.RefreshTokens.AddAsync(token, ct);
        
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task RevokeAllForUserAsync(UserId userId, CancellationToken ct = default)
    {
        // Bulk revoke - used on logout to invalidate all sessions for this user
        await dbContext.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.IsRevoked, true), ct);
    }
}