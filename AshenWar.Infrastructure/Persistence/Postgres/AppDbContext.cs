using AshenWar.Domain.Entities.Users;
using AshenWar.Domain.ValueObjects.Identifiers.Players;
using AshenWar.Domain.ValueObjects.Identifiers.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AshenWar.Infrastructure.Persistence.Postgres;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Value converters for custom ID types
        var userIdConverter = new ValueConverter<UserId, Guid>(
            id => id.Value,
            guid => new UserId(guid)
        );

        var unitDefIdConverter = new ValueConverter<UnitDefinitionId, Guid>(
            id => id.Value,
            guid => new UnitDefinitionId(guid)
        );
        
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasConversion(userIdConverter);

            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.HasIndex(u => u.Email).IsUnique();

            e.Property(u => u.Username).IsRequired().HasMaxLength(64);
            e.HasIndex(u => u.Username).IsUnique();

            e.Property(u => u.PasswordHash).HasMaxLength(512);

            e.Property(u => u.Role).IsRequired().HasMaxLength(32);

            // Units owned - stored as a JSON column (simple list of Guids)
            e.Property(u => u.Units).HasConversion(
                list => string.Join(',', list.Select(id => id.ToString())),
                raw => raw == ""
                    ? new List<UnitDefinitionId>()
                    : raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => new UnitDefinitionId(Guid.Parse(s)))
                        .ToList()
            ).HasColumnName("owned_units")
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<UnitDefinitionId>>(
                (a, b) => a != null && b != null && a.SequenceEqual(b),
                list => list.Aggregate(0, (hash, id) =>
                    HashCode.Combine(hash, id.Value.GetHashCode())),
                list => list.ToList()
            ));
        });
        
        // RefreshToken
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(t => t.Id);

            e.Property(t => t.UserId).HasConversion(userIdConverter).IsRequired();

            e.Property(t => t.TokenHash).IsRequired().HasMaxLength(512);
            e.HasIndex(t => t.TokenHash).IsUnique();

            e.HasIndex(t => t.UserId); // fast lookup by user on logout
        });
    }
}